//
//  BackupViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 05.05.2026.
//

import Combine
import Photos
import SwiftUI

final class BackupViewModel: ObservableObject {
    @Published var thumbnails: [UIImage] = []
    @Published var isLoading = false
    @Published var uploadStatus: String? = nil
    @Published var uploadingAssetID: String? = nil
    @Published var isAutoUploading = false
    
    private var errorInfo: String? = nil
    private var allAssets: [PHAsset] = []
    private var currentIndex = 0
    private let batchSize = 100

    var uploadService: UploadService
    var testService: TestService
    private let settings = SettingsService()

    init(
        uploadService: UploadService? = nil,
        testService: TestService? = nil
    ) {
        self.uploadService = uploadService ?? UploadService(
            uploadApi: UploadApiClient(baseURL: URL(string: "http://localhost")!)
        )

        self.testService = testService ?? TestService(
            testApi: TestApiClient(baseURL: URL(string: "http://localhost")!)
        )
    }

    func restartIfNeeded() {
        if settings.backupEnabled && !isAutoUploading {
            startAutoUploadLoop()
        }
    }

    func loadPhotos() {
        isLoading = true

        let status = PHPhotoLibrary.authorizationStatus(for: .readWrite)

        switch status {
        case .notDetermined:
            PHPhotoLibrary.requestAuthorization(for: .readWrite) { newStatus in
                DispatchQueue.main.async {
                    if newStatus == .authorized || newStatus == .limited {
                        self.prepareAssets()
                    } else {
                        self.isLoading = false
                    }
                }
            }

        case .authorized, .limited:
            prepareAssets()

        default:
            isLoading = false
        }
    }

    private func prepareAssets() {
        let fetchOptions = PHFetchOptions()
        fetchOptions.sortDescriptors = [
            NSSortDescriptor(key: "creationDate", ascending: true)
        ]

        let fetched = PHAsset.fetchAssets(with: .image, options: fetchOptions)

        let uploaded = Set(settings.uploadedPhotoIDs)
        let failed = Set(settings.failedPhotoIDs)

        var filtered: [PHAsset] = []
        fetched.enumerateObjects { asset, _, _ in
            if uploaded.contains(asset.localIdentifier) == false &&
               failed.contains(asset.localIdentifier) == false {
                filtered.append(asset)
            }
        }

        self.allAssets = filtered
        self.currentIndex = 0

        loadNextBatch()
    }

    func loadNextBatch() {
        guard currentIndex < allAssets.count else { return }

        isLoading = true

        let manager = PHImageManager.default()

        let requestOptions = PHImageRequestOptions()
        requestOptions.deliveryMode = .fastFormat
        requestOptions.resizeMode = .fast
        requestOptions.isNetworkAccessAllowed = true

        let endIndex = min(currentIndex + batchSize, allAssets.count)

        DispatchQueue.global(qos: .userInitiated).async {
            var newImages: [UIImage] = []

            for i in self.currentIndex..<endIndex {
                let asset = self.allAssets[i]

                let semaphore = DispatchSemaphore(value: 0)

                manager.requestImage(
                    for: asset,
                    targetSize: CGSize(width: 200, height: 200),
                    contentMode: .aspectFill,
                    options: requestOptions
                ) { image, _ in
                    if let image = image {
                        newImages.append(image)
                    }
                    semaphore.signal()
                }

                semaphore.wait()
            }

            DispatchQueue.main.async {
                self.thumbnails.append(contentsOf: newImages)
                self.currentIndex = endIndex
                self.isLoading = false
            }
        }
    }

    func assetForThumbnail(index: Int) -> PHAsset? {
        guard index < allAssets.count else { return nil }
        return allAssets[index]
    }

    func startAutoUploadLoop() {
        guard isAutoUploading == false else { return }
        isAutoUploading = true

        Task {
            while isAutoUploading {

                if KeychainService.load("jwt") == nil {
                    await MainActor.run { self.uploadStatus = "Nepřihlášený uživatel – upload zastaven" }
                    break
                }

                if settings.backupEnabled == false {
                    await MainActor.run {
                        let helper = errorInfo == nil ? "" : "\(errorInfo!)"
                        self.uploadStatus = helper + "Zálohování je vypnuto"
                    }
                    break
                }

                let uploaded = Set(settings.uploadedPhotoIDs)
                let failed = Set(settings.failedPhotoIDs)

                guard let asset = allAssets.first(where: {
                    uploaded.contains($0.localIdentifier) == false &&
                    failed.contains($0.localIdentifier) == false
                }) else {
                    await MainActor.run { self.uploadStatus = "Všechny fotky jsou nahrané" }
                    break
                }

                await uploadSingleAsset(asset)

                await MainActor.run {
                    self.thumbnails.removeAll()
                }
                loadPhotos()

                try? await Task.sleep(nanoseconds: 300_000_000)
            }

            isAutoUploading = false
        }
    }

    func uploadSingleAsset(_ asset: PHAsset) async {
        await MainActor.run {
            self.uploadingAssetID = asset.localIdentifier
        }

        guard await checkServerAvailability() else { return }

        let manager = PHImageManager.default()
        let options = PHImageRequestOptions()
        options.isNetworkAccessAllowed = true
        options.deliveryMode = .highQualityFormat
        options.resizeMode = .none

        await withCheckedContinuation { continuation in
            manager.requestImage(
                for: asset,
                targetSize: PHImageManagerMaximumSize,
                contentMode: .default,
                options: options
            ) { image, info in

                guard let image else {
                    Task { @MainActor in self.handleFailedAsset(asset, reason: "Nelze načíst bitmapu") }
                    continuation.resume()
                    return
                }

                guard let jpegData = image.jpegData(compressionQuality: 0.95) else {
                    Task { @MainActor in self.handleFailedAsset(asset, reason: "Nelze převést na JPEG") }
                    continuation.resume()
                    return
                }

                Task {
                    await self.uploadOriginalJPEG(jpegData)
                    continuation.resume()
                }
            }
        }
    }

    private func checkServerAvailability() async -> Bool {
        while true {
            do {
                let response = try await testService.runTest()
                if response.success {
                    await MainActor.run { }
                    return true
                } else {
                    await MainActor.run { self.uploadStatus = "Server odpověděl, ale success = false" }
                }
            } catch {
                await MainActor.run {
                    self.uploadStatus = "Server je nedostupný – čekám 10 sekund"
                }
            }

            try? await Task.sleep(nanoseconds: 10_000_000_000)
        }
    }

    @MainActor
    private func uploadOriginalJPEG(_ data: Data) async {
        let fileName = "backup_\(UUID().uuidString).jpg"

        do {
            if settings.backupFamilyMode {
                try await uploadService.uploadFamilyImage(imageData: data, fileName: fileName)
            } else {
                try await uploadService.uploadPersonalImage(imageData: data, fileName: fileName)
            }

            self.uploadStatus = "Nahráno (\(fileName))"
            errorInfo = nil

            if let id = self.uploadingAssetID {
                var uploaded = settings.uploadedPhotoIDs
                uploaded.append(id)
                settings.uploadedPhotoIDs = uploaded
            }

        } catch {
            let message = error.localizedDescription.lowercased()

            if message.contains("401") || message.contains("unauthorized") {
                self.uploadStatus = "Nepřihlášený – upload pozastaven"
                return
            }

            await handleFailedAssetID(self.uploadingAssetID, reason: error.localizedDescription)
        }

        self.uploadingAssetID = nil
    }

    @MainActor
    private func handleFailedAsset(_ asset: PHAsset, reason: String) {
        self.uploadStatus = " Chyba: \(reason)"
        errorInfo = " Chyba: \(reason)"

        var failed = settings.failedPhotoIDs
        failed.append(asset.localIdentifier)
        settings.failedPhotoIDs = failed
        settings.backupEnabled = false
    }

    @MainActor
    private func handleFailedAssetID(_ id: String?, reason: String) async {
        self.uploadStatus = " Chyba uploadu: \(reason)"
        errorInfo = " Chyba uploadu: \(reason)"
        
        if await checkServerAvailability() {
            // blacklist
            if let id {
                var failed = settings.failedPhotoIDs
                failed.append(id)
                settings.failedPhotoIDs = failed
            }
            
            settings.backupEnabled = false
        }
    }
}
