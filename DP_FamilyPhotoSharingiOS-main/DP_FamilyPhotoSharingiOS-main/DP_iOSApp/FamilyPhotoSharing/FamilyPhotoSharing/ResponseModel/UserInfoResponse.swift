//
//  UserInfoResponse.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//
typealias UserInfoResponse = ApiResponse<UserInfo>

struct UserInfo: Codable {
    let name: String
    let surname: String
    let login: String
    let groupName: String
    let roleId: Int
}
