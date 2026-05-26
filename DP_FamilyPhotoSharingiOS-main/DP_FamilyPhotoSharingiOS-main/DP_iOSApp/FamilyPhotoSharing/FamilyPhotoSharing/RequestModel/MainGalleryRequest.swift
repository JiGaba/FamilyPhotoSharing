//
//  MainGalleryRequest.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

struct MainGalleryRequest: Codable {
    let page: Int

    enum CodingKeys: String, CodingKey {
        case page = "Page"
    }
}
