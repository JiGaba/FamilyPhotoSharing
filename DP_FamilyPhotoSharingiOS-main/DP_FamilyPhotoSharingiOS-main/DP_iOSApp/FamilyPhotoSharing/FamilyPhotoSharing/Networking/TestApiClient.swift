//
//  TestApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 25.05.2026.
//

import Foundation

protocol TestApiProtocol {
    func runTest() async throws -> ApiResponse<String>
}

class TestApiClient: TestApiProtocol {
    private let client: ApiClient
    private let testUrl = "/api/test/run"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func runTest() async throws -> ApiResponse<String> {
        try await client.get(testUrl)
    }
}
