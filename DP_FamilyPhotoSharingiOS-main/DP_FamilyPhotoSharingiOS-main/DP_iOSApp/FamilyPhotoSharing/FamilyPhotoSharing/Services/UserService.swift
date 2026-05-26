//
//  UserService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

class UserService {
    private let userApi: UserApiProtocol

    init(userApi: UserApiProtocol) {
        self.userApi = userApi
    }

    func loadUserInfo() async throws -> UserInfo {
        let response = try await userApi.getUserInfo()
        return response.data
    }
}
