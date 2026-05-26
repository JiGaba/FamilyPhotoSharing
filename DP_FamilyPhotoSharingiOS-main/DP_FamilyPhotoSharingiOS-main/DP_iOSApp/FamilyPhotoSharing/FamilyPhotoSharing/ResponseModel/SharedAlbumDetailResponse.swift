//
//  SharedAlbumDetailResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

typealias SharedAlbumDetailResponse = ApiResponse<SharedAlbumDetail>

struct SharedAlbumDetail: Codable {
    let id: Int
    let albumName: String
    let albumDescription: String
    let titlePhotoId: Int
    let createDateTime: String
    let createAuthor: Int
    let ownerUserId: Int
    let userGroupsId: Int
}
