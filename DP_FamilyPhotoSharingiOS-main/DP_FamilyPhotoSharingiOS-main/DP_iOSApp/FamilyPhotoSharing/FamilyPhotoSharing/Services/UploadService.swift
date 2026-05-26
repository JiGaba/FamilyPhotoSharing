//
//  UploadService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation

class UploadService {
    private let uploadApi: UploadApiProtocol

    init(uploadApi: UploadApiProtocol) {
        self.uploadApi = uploadApi
    }

    func uploadPersonalImage(imageData: Data, fileName: String) async throws {
        let response = try await uploadApi.uploadPersonalImage(
            imageData: imageData,
            fileName: fileName
        )

        guard response.success else {
            throw ApiError.server(message: response.message ?? "")
        }

        guard response.data == true else {
            throw ApiError.server(message: "Upload failed")
        }
    }

    func uploadFamilyImage(imageData: Data, fileName: String) async throws {
        let response = try await uploadApi.uploadFamilyImage(
            imageData: imageData,
            fileName: fileName
        )

        guard response.success else {
            throw ApiError.server(message: response.message ?? "")
        }

        guard response.data == true else {
            throw ApiError.server(message: "Upload failed")
        }
    }
}

