//
//  PhotoApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation

protocol PhotoApiProtocol {
    func getMainPersonalAlbum(request: MainGalleryRequest) async throws -> PhotoResponse
    func getMainGroupAlbum(request: MainGalleryRequest) async throws -> PhotoResponse
    func getMainGroupAlbumOwnPhoto(request: MainGalleryRequest) async throws -> PhotoResponse
    func showAlbum(request: GalleryRequest) async throws -> PhotoResponse
    func showGroupAlbum(request: GalleryRequest) async throws -> PhotoResponse
    func showMySharedAlbum(request: GalleryRequest) async throws -> PhotoResponse
    func showSharedAlbum(request: GalleryRequest) async throws -> PhotoResponse
    func loadPhotos(request: LoadPhotosRequest) async throws -> LoadPhotosResponse
    func getFullPhoto(request: GetPhotoRequest) async throws -> (data: Data, mimeType: String?)
    func getThumbnail(request: GetPhotoRequest) async throws -> (data: Data, mimeType: String?)
}


class PhotoApiClient: PhotoApiProtocol {
    private let client: ApiClient

    private let mainPersonalAlbumUrl = "/api/photo/GetMainPersonalAlbum"
    private let mainGroupAlbumUrl = "/api/photo/GetMainGroupAlbum"
    private let mainGroupAlbumOwnPhotoUrl = "/api/photo/MainGroupAlbumOwnPhoto"
    private let showAlbumUrl = "/api/photo/ShowAlbum"
    private let showGroupAlbumUrl = "/api/photo/ShowGroupAlbum"
    private let showMySharedAlbumUrl = "/api/photo/ShowMySharedAlbum"
    private let showSharedAlbumUrl = "/api/photo/ShowSharedAlbum"
    private let loadPhotosUrl = "/api/photo/LoadPhotos"
    private let getFullPhotoUrl = "/api/photo/GetFullPhoto"
    private let getThumbnailUrl = "/api/photo/GetThumbnail"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func getMainPersonalAlbum(request: MainGalleryRequest) async throws -> PhotoResponse {
        try await client.post(mainPersonalAlbumUrl, body: request)
    }

    func getMainGroupAlbum(request: MainGalleryRequest) async throws -> PhotoResponse {
        try await client.post(mainGroupAlbumUrl, body: request)
    }

    func getMainGroupAlbumOwnPhoto(request: MainGalleryRequest) async throws -> PhotoResponse {
        try await client.post(mainGroupAlbumOwnPhotoUrl, body: request)
    }

    func showAlbum(request: GalleryRequest) async throws -> PhotoResponse {
        try await client.post(showAlbumUrl, body: request)
    }

    func showGroupAlbum(request: GalleryRequest) async throws -> PhotoResponse {
        try await client.post(showGroupAlbumUrl, body: request)
    }

    func showMySharedAlbum(request: GalleryRequest) async throws -> PhotoResponse {
        try await client.post(showMySharedAlbumUrl, body: request)
    }

    func showSharedAlbum(request: GalleryRequest) async throws -> PhotoResponse {
        try await client.post(showSharedAlbumUrl, body: request)
    }

    func loadPhotos(request: LoadPhotosRequest) async throws -> LoadPhotosResponse {
        try await client.post(loadPhotosUrl, body: request)
    }

    func getFullPhoto(request: GetPhotoRequest) async throws -> (data: Data, mimeType: String?) {
        try await client.postDataWithMime(getFullPhotoUrl, body: request)
    }

    func getThumbnail(request: GetPhotoRequest) async throws -> (data: Data, mimeType: String?) {
        try await client.postDataWithMime(getThumbnailUrl, body: request)
    }
}
