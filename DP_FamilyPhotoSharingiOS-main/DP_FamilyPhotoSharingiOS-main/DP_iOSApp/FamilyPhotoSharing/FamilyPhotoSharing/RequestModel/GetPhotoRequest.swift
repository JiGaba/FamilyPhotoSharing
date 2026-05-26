//
//  GetPhotoRequest.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

struct GetPhotoRequest: Codable {
    let photoId: Int

    enum CodingKeys: String, CodingKey {
        case photoId = "PhotoId"
    }
}
