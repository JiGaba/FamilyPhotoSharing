//
//  LoginApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

import Foundation

protocol LoginApiProtocol {
    func login(login: String, password: String) async throws -> LoginResponse
    func refresh(token: String) async throws -> RefreshResponse
}

class LoginApiClient: LoginApiProtocol {
    private let client: ApiClient
    private let loginUrl = "/api/auth/login"
    private let refreshUrl = "/api/auth/refresh"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func login(login: String, password: String) async throws -> LoginResponse {
        let body = LoginRequest(login: login, password: password)
        return try await client.post(loginUrl, body: body)
    }

    func refresh(token: String) async throws -> RefreshResponse {
        let body = RefreshRequest(token: token)
        return try await client.post(refreshUrl, body: body)
    }
}

