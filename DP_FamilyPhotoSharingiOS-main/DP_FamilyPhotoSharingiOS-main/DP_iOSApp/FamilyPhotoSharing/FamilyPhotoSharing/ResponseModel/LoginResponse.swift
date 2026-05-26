//
//  LoginResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

struct LoginResponse: Codable {
    let token: String
    let refreshToken: RefreshToken
}

struct RefreshToken: Codable {
    let id: Int
    let userId: Int
    let token: String
    let expires: String
    let isRevoked: Bool
    let createAuthorId: Int
}

