//
//  PhotoGalleryInfo.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

struct PhotoGalleryInfo: Codable {
    let galleryName: String
    let galleryDescription: String
    let titlePotoId: Int
    let thumbnailData: Int
    let albumId: Int
    let userRole: Int
    let photos: [PhotoItem]
}
