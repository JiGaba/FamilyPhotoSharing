//
//  LoadPhotosRequest.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

struct LoadPhotosRequest: Codable {
    let thumbnailData: Int
    let page: Int
    let part: Int
    let albumId: Int

    enum CodingKeys: String, CodingKey {
        case thumbnailData = "ThumbnailData"
        case page = "Page"
        case part = "Part"
        case albumId = "AlbumId"
    }
}
