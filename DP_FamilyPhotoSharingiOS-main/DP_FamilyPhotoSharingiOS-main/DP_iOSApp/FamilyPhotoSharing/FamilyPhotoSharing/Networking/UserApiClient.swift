//
//  UserApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

import Foundation

protocol UserApiProtocol {
    func getUserInfo() async throws -> UserInfoResponse
}

class UserApiClient: UserApiProtocol {
    private let client: ApiClient
    private let userInfoUrl = "/api/auth/getuserinfo"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func getUserInfo() async throws -> UserInfoResponse {
        try await client.get(userInfoUrl)
    }
}
