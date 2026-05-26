//
//  PhotoDetailViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//

import Combine
import Foundation

final class PhotoDetailViewModel: ObservableObject {
    @Published var fullImage: Data?
    @Published var mimeType: String?
    @Published var isLoading = false

    private let service: PhotoService
    private let photoId: Int

    init(photoId: Int, service: PhotoService) {
        self.photoId = photoId
        self.service = service
    }

    @MainActor
    func load() async {
        isLoading = true
        defer { isLoading = false }

        let request = GetPhotoRequest(photoId: photoId)

        guard let result = try? await service.loadFullPhoto(request: request) else {
            self.fullImage = nil
            self.mimeType = nil
            return
        }

        self.fullImage = result.data
        self.mimeType = result.mimeType
    }
}
