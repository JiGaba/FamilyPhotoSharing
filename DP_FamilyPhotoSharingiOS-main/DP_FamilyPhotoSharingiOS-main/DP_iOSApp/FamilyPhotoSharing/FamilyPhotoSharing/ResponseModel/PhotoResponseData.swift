//
//  PhotoResponseData.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//
typealias PhotoResponse = ApiResponse<PhotoResponseData>


struct PhotoResponseData: Codable {
    let thumbnailData: Int
    let page: Int
    let pageSize: Int
    let totalCount: Int
    let gallery: PhotoGalleryInfo
}
