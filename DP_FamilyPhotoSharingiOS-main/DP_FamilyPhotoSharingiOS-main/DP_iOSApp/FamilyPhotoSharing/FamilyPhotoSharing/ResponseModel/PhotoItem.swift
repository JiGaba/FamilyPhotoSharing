//
//  PhotoItem.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

typealias LoadPhotosResponse = ApiResponse<[PhotoItem]>

struct PhotoItem: Codable, Identifiable {
    let id: Int
    let photoName: String
    let photoDescription: String
    let ownerId: Int
    let groupsId: Int
    let fileSize: Int
    let fsFileName: String
    let personal: Bool
    let createAuthor: Int
    let createDateTime: String
    let data: String?
}
