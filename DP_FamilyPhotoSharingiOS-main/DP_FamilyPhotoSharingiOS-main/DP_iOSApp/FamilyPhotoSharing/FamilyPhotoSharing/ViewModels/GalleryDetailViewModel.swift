//  GalleryDetailViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//

import Foundation
import Combine
import SwiftUI

final class GalleryDetailViewModel: ObservableObject {
    @Published var album: AlbumDetailModel
    @Published var photos: PhotoResponseData?
    @Published var galleryPhotos: [GalleryPhoto] = []
    @Published var isLoading = false
    @Published var isLoadingMore = false
    @Published var selectedPhoto: GalleryPhoto?

    private let repository: PhotoRepositoryProtocol
    let service: PhotoService

    private var currentPage = 1
    private var currentPart = 1
    private let maxParts = 5
    private var totalCount = 0

    private var didInitialLoad = false

    init(album: AlbumDetailModel, repository: PhotoRepositoryProtocol, service: PhotoService) {
        self.album = album
        self.repository = repository
        self.service = service
    }
    
    @MainActor
    func load() async {
        guard !didInitialLoad else { return }
        didInitialLoad = true

        let cacheKey = CacheFile.galleryMetadata.filename(
            id: album.id,
            albumType: album.type
        )


        // 1) CACHE FIRST – jen pokud galerie je prázdná
        var cachedItems: [GalleryPhoto] = []

        if let cached: PhotoResponseData = CacheManager.shared.load(
            PhotoResponseData.self,
            from: cacheKey
        ), self.galleryPhotos.isEmpty {

            cachedItems = cached.gallery.photos.map { GalleryPhoto(id: $0.id) }
            self.galleryPhotos = cachedItems
            await loadThumbnailsParallel(for: cachedItems)
        }

        // 2) API – master data
        isLoading = true
        defer { isLoading = false }

        guard let response = await repository.loadInitialGallery(album: album) else { return }

        if cachedItems.map(\.id) == response.gallery.photos.map(\.id) {
            print("Cache == API → UI se nepřepisuje")
            self.totalCount = response.totalCount
            return
        }

        CacheManager.shared.save(response, as: cacheKey)

        self.photos = response
        self.totalCount = response.totalCount

        let freshItems = response.gallery.photos.map { GalleryPhoto(id: $0.id) }
        self.galleryPhotos = freshItems

        self.currentPage = 1
        self.currentPart = 1

        //print("Načteno API, totalCount = \(totalCount), items = \(freshItems.count)")

        await loadThumbnailsParallel(for: freshItems)
    }
    
    @MainActor
    func loadMore() async {
        guard !isLoadingMore else { return }

        guard totalCount == 0 || galleryPhotos.count < totalCount else {
            print("Není co načítat")
            return
        }

        isLoadingMore = true

        currentPart += 1
        if currentPart > maxParts {
            currentPart = 1
            currentPage += 1
        }

        //print("Načítám další část: page=\(currentPage), part=\(currentPart)")

        let items = await repository.loadPhotosPage(
            album: album,
            page: currentPage,
            part: currentPart
        )

        let newItems = items.map { GalleryPhoto(id: $0.id) }

        let filtered = newItems.filter { newItem in
            !galleryPhotos.contains(where: { $0.id == newItem.id })
        }

        galleryPhotos.append(contentsOf: filtered)

        //print("Load next part OK, přidáno \(filtered.count) fotek, celkem \(galleryPhotos.count)")

        await loadThumbnailsParallel(for: filtered)
        isLoadingMore = false
    }

    func loadThumbnailsParallel(for items: [GalleryPhoto]) async {
        await withTaskGroup(of: (Int, Data?).self) { group in
            for item in items {
                group.addTask { [service] in
                    let cacheName = CacheFile.thumbnail.filename(id: item.id)

                    if let cached: Data = await CacheManager.shared.load(Data.self, from: cacheName) {
                        return (item.id, cached)
                    }

                    let request = GetPhotoRequest(photoId: item.id)
                    do {
                        let (data, _) = try await service.loadThumbnail(request: request)
                        await CacheManager.shared.save(data, as: cacheName)
                        return (item.id, data)
                    } catch {
                        print("Thumbnail error for \(item.id): \(error)")
                        return (item.id, nil)
                    }
                }
            }

            for await (id, data) in group {
                await MainActor.run {
                    if let index = self.galleryPhotos.firstIndex(where: { $0.id == id }) {
                        self.galleryPhotos[index].thumbnail = data
                    }
                }
            }
        }
    }

    @MainActor
    func openPhoto(_ photo: GalleryPhoto) {
        selectedPhoto = photo
    }
}
