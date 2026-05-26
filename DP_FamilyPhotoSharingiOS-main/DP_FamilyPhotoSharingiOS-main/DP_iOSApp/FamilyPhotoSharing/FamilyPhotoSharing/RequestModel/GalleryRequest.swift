//
//  GalleryRequest.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

struct GalleryRequest: Codable {
    let page: Int
    let albumId: Int

    enum CodingKeys: String, CodingKey {
        case page = "Page"
        case albumId = "AlbumId"
    }
}
