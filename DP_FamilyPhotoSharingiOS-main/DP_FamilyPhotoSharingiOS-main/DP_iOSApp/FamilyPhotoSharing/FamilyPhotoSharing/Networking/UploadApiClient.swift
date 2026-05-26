//
//  UploadApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation

protocol UploadApiProtocol {
    func uploadPersonalImage(imageData: Data, fileName: String) async throws -> ApiResponse<Bool>
    func uploadFamilyImage(imageData: Data, fileName: String) async throws -> ApiResponse<Bool>
}

class UploadApiClient: UploadApiProtocol {

    private let client: ApiClient
    private let uploadUrl = "/api/upload/UploadPersonal"
    private let uploadFamilyUrl = "/api/upload/UploadGroup"

    init(baseURL: URL) {
        self.client = ApiClient(baseURL: baseURL)
    }

    func uploadPersonalImage(imageData: Data, fileName: String) async throws -> ApiResponse<Bool> {
        try await client.uploadMultipart(
            uploadUrl,
            fileName: fileName,
            fileData: imageData
        )
    }
    
    func uploadFamilyImage(imageData: Data, fileName: String) async throws -> ApiResponse<Bool> {
        try await client.uploadMultipart(
            uploadFamilyUrl,
            fileName: fileName,
            fileData: imageData
        )
    }
}
