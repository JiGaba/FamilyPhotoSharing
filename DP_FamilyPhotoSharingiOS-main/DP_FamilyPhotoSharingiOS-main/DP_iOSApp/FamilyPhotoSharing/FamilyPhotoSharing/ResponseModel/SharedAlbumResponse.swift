//
//  SharedAlbumResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//
typealias SharedAlbumsResponse = ApiResponse<[SharedAlbum]>

struct SharedAlbum: Codable, Identifiable {
    let id: Int
    let albumName: String
    let albumDescription: String
    let photoCount: Int
    let selected: Bool?

    let createAuthorName: String
    let createAuthor: Int
    let ownerUserId: Int
    let userGroupsId: Int

    let titlePhotoId: Int
    let createDateTime: String

    // sharing navíc
    let hostUserId: Int
    let hostUserCount: Int
    let hostUserIds: [Int]?
}
