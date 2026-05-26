//
//  ApiError.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

enum ApiError: Error {
    case server(message: String)
    case emptyData
    case unauthorized
    case decodingError
    case unknown
}

