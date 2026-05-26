//
//  SharedAlbumService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

class SharedAlbumService {
    private let sharedAlbumApi: SharedAlbumApiProtocol

    init(sharedAlbumApi: SharedAlbumApiProtocol) {
        self.sharedAlbumApi = sharedAlbumApi
    }

    func loadMySharedAlbums() async throws -> [SharedAlbum] {
        let response = try await sharedAlbumApi.getMySharedAlbums()
        return response.data
    }

    func loadSharedAlbums() async throws -> [SharedAlbum] {
        let response = try await sharedAlbumApi.getSharedAlbums()
        return response.data
    }

    func loadSharedAlbumDetail(id: Int) async throws -> SharedAlbumDetail {
        let response = try await sharedAlbumApi.getSharedAlbumDetail(id: id)
        return response.data
    }
}

