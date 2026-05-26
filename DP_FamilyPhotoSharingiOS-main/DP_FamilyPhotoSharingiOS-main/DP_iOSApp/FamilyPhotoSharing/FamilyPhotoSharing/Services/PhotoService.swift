//
//  PhotoService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation

class PhotoService {
    private let photoApi: PhotoApiProtocol

    init(photoApi: PhotoApiProtocol) {
        self.photoApi = photoApi
    }

    func loadMainPersonalAlbum(request: MainGalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.getMainPersonalAlbum(request: request)
        print(response.success)
        print(response.message ?? "")
        return response.data
    }

    func loadMainGroupAlbum(request: MainGalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.getMainGroupAlbum(request: request)
        return response.data
    }

    func loadMainGroupAlbumOwnPhoto(request: MainGalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.getMainGroupAlbumOwnPhoto(request: request)
        return response.data
    }

    func showAlbum(request: GalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.showAlbum(request: request)
        return response.data
    }

    func showGroupAlbum(request: GalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.showGroupAlbum(request: request)
        return response.data
    }

    func showMySharedAlbum(request: GalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.showMySharedAlbum(request: request)
        return response.data
    }

    func showSharedAlbum(request: GalleryRequest) async throws -> PhotoResponseData {
        let response = try await photoApi.showSharedAlbum(request: request)
        return response.data
    }

    func loadPhotos(request: LoadPhotosRequest) async throws -> LoadPhotosResponse {
        try await photoApi.loadPhotos(request: request)
    }

    func loadFullPhoto(request: GetPhotoRequest) async throws -> (data: Data, mimeType: String?) {
        try await photoApi.getFullPhoto(request: request)
    }

    func loadThumbnail(request: GetPhotoRequest) async throws -> (data: Data, mimeType: String?) {
        try await photoApi.getThumbnail(request: request)
    }
}
