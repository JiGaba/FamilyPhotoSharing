//
//  AlbumApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation

protocol AlbumApiProtocol {
    func getPrivateAlbums() async throws -> AlbumsResponse
    func getFamilyAlbums() async throws -> FamilyAlbumsResponse
    func getAlbumDetail(id: Int) async throws -> AlbumDetailResponse
    func getFamilyAlbumDetail(id: Int) async throws -> FamilyAlbumDetailResponse
}

class AlbumApiClient: AlbumApiProtocol {
    private let client: ApiClient

    private let privateAlbumsUrl = "/api/album/getPersonalPhotoAlbums"
    private let familyAlbumsUrl = "/api/album/GetGroupPhotoAlbums"
    private let albumDetailUrl = "/api/album/GetPersonalPhotoAlbum"
    private let familyAlbumDetailUrl = "/api/album/GetGroupPhotoAlbum"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func getPrivateAlbums() async throws -> AlbumsResponse {
        try await client.get(privateAlbumsUrl)
    }

    func getFamilyAlbums() async throws -> FamilyAlbumsResponse {
        try await client.get(familyAlbumsUrl)
    }

    func getAlbumDetail(id: Int) async throws -> AlbumDetailResponse {
        let request = AlbumIdRequest(albumId: id)
        return try await client.post(albumDetailUrl, body: request)
    }

    func getFamilyAlbumDetail(id: Int) async throws -> FamilyAlbumDetailResponse {
        let request = AlbumIdRequest(albumId: id)
        return try await client.post(familyAlbumDetailUrl, body: request)
    }
}
