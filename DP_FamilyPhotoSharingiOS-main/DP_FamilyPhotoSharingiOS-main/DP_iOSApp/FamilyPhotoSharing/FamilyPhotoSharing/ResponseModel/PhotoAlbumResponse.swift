//
//  PhotoAlbumResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation

typealias AlbumsResponse = ApiResponse<[Album]>        // seznam soukromých alb
typealias FamilyAlbumsResponse = ApiResponse<[Album]>  // seznam rodinných alb
typealias AlbumDetailResponse = ApiResponse<Album>     // detail alba
typealias FamilyAlbumDetailResponse = ApiResponse<Album>     // detail rodinného alba

struct Album: Codable, Identifiable {
    let photoCount: Int
    let authorName: String
    let selected: Bool?
    let id: Int
    let albumName: String
    let albumDescription: String
    let personal: Bool
    let titlePhotoId: Int
    let createDateTime: String
    let createAuthor: Int
    let ownerUserId: Int
    let userGroupsId: Int

    enum CodingKeys: String, CodingKey {
        case photoCount
        case createAuthorName
        case iAuthorName
        case selected
        case id, albumName, albumDescription, personal, titlePhotoId
        case createDateTime, createAuthor, ownerUserId, userGroupsId
        case authorName // přidáme pro encode
    }

    init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)

        photoCount = try container.decode(Int.self, forKey: .photoCount)
        id = try container.decode(Int.self, forKey: .id)
        albumName = try container.decode(String.self, forKey: .albumName)
        albumDescription = try container.decode(String.self, forKey: .albumDescription)
        personal = try container.decode(Bool.self, forKey: .personal)
        titlePhotoId = try container.decode(Int.self, forKey: .titlePhotoId)
        createDateTime = try container.decode(String.self, forKey: .createDateTime)
        createAuthor = try container.decode(Int.self, forKey: .createAuthor)
        ownerUserId = try container.decode(Int.self, forKey: .ownerUserId)
        userGroupsId = try container.decode(Int.self, forKey: .userGroupsId)
        selected = try container.decodeIfPresent(Bool.self, forKey: .selected)

        authorName =
                    try container.decodeIfPresent(String.self, forKey: .createAuthorName) ??
                    container.decodeIfPresent(String.self, forKey: .iAuthorName) ?? ""
    }

    func encode(to encoder: Encoder) throws {
        var container = encoder.container(keyedBy: CodingKeys.self)

        try container.encode(photoCount, forKey: .photoCount)
        try container.encode(id, forKey: .id)
        try container.encode(albumName, forKey: .albumName)
        try container.encode(albumDescription, forKey: .albumDescription)
        try container.encode(personal, forKey: .personal)
        try container.encode(titlePhotoId, forKey: .titlePhotoId)
        try container.encode(createDateTime, forKey: .createDateTime)
        try container.encode(createAuthor, forKey: .createAuthor)
        try container.encode(ownerUserId, forKey: .ownerUserId)
        try container.encode(userGroupsId, forKey: .userGroupsId)
        try container.encode(selected, forKey: .selected)
        try container.encode(authorName, forKey: .authorName)
    }
}
