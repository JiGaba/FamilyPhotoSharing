//
//  ApiResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

struct ApiResponse<T: Decodable>: Decodable {
    let success: Bool
    let message: String?
    let errors: String?
    let data: T
}
