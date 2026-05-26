//
//  AlbumService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

class AlbumService {
    private let albumApi: AlbumApiProtocol

    init(albumApi: AlbumApiProtocol) {
        self.albumApi = albumApi
    }

    func loadPrivateAlbums() async throws -> [Album] {
        let response = try await albumApi.getPrivateAlbums()
        return response.data
    }

    func loadFamilyAlbums() async throws -> [Album] {
        let response = try await albumApi.getFamilyAlbums()
        return response.data
    }

    func loadAlbumDetail(id: Int) async throws -> Album {
        let response = try await albumApi.getAlbumDetail(id: id)
        return response.data
    }

    func loadFamilyAlbumDetail(id: Int) async throws -> Album {
        let response = try await albumApi.getFamilyAlbumDetail(id: id)
        return response.data
    }
}
