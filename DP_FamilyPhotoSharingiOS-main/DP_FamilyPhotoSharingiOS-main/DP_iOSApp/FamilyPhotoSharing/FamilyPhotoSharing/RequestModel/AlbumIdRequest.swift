//
//  AlbumIdRequest.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

struct AlbumIdRequest: Codable {
    let albumId: Int

    enum CodingKeys: String, CodingKey {
        case albumId = "AlbumId"
    }
}
