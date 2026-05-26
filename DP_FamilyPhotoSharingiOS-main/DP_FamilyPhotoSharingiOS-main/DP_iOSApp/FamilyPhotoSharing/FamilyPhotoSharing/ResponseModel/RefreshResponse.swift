//
//  RefreshResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 20.05.2026.
//

struct RefreshResponse: Codable {
    let token: String
    let refreshToken: String
}
