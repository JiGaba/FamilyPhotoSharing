//
//  SharedAlbumApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//
import Foundation

protocol SharedAlbumApiProtocol {
    func getMySharedAlbums() async throws -> SharedAlbumsResponse
    func getSharedAlbums() async throws -> SharedAlbumsResponse
    func getSharedAlbumDetail(id: Int) async throws -> SharedAlbumDetailResponse
}

class SharedAlbumApiClient: SharedAlbumApiProtocol {

    private let client: ApiClient

    private let mySharedAlbumsUrl = "/api/sharedAlbum/GetMySharedPhotoAlbums"
    private let sharedAlbumsUrl = "/api/sharedAlbum/GetSharedPhotoAlbums"
    private let sharedAlbumDetailUrl = "/api/sharedAlbum/GetSharedPhotoAlbum"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func getMySharedAlbums() async throws -> SharedAlbumsResponse {
        try await client.get(mySharedAlbumsUrl)
    }
    
    func getSharedAlbums() async throws -> SharedAlbumsResponse {
        try await client.get(sharedAlbumsUrl)
    }

    func getSharedAlbumDetail(id: Int) async throws -> SharedAlbumDetailResponse {
        try await client.get("\(sharedAlbumDetailUrl)/\(id)")
    }
}
