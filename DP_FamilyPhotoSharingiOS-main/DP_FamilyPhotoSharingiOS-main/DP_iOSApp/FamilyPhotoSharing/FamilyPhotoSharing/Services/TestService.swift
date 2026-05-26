//
//  TestService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 25.05.2026.
//

class TestService {
    private let testApi: TestApiProtocol

    init(testApi: TestApiProtocol) {
        self.testApi = testApi
    }

    func runTest() async throws -> ApiResponse<String> {
        
        let result = try await testApi.runTest()
        return result
    }
}
