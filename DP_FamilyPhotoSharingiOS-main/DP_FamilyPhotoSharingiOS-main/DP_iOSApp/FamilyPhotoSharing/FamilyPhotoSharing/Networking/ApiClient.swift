//
//  ApiClient.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

import Foundation

class ApiClient {
    let baseURL: URL

    init(baseURL: URL) {
        self.baseURL = baseURL
    }

    func get<U: Decodable>(_ path: String) async throws -> U {
        do {
            return try await performGet(path)
        } catch ApiError.unauthorized {
            guard try await tryRefreshToken() else {
                throw ApiError.unauthorized
            }
            return try await performGet(path)
        }
    }

    func post<T: Codable, U: Decodable>(_ path: String, body: T) async throws -> U {
        do {
            return try await performPost(path, body: body)
        } catch ApiError.unauthorized {
            guard try await tryRefreshToken() else {
                throw ApiError.unauthorized
            }
            return try await performPost(path, body: body)
        }
    }

    func uploadMultipart<U: Decodable>(
        _ path: String,
        fileName: String,
        fileData: Data
    ) async throws -> U {

        do {
            return try await performMultipart(path, fileName: fileName, fileData: fileData)
        } catch ApiError.unauthorized {
            guard try await tryRefreshToken() else {
                throw ApiError.unauthorized
            }
            return try await performMultipart(path, fileName: fileName, fileData: fileData)
        }
    }

    func postDataWithMime<T: Encodable>(_ path: String, body: T) async throws -> (data: Data, mimeType: String?) {
        do {
            return try await performPostData(path, body: body)
        } catch ApiError.unauthorized {
            guard try await tryRefreshToken() else {
                throw ApiError.unauthorized
            }
            return try await performPostData(path, body: body)
        }
    }

    private func performGet<U: Decodable>(_ path: String) async throws -> U {
        let url = baseURL.appendingPathComponent(path)

        var request = URLRequest(url: url)
        request.httpMethod = "GET"

        if let token = KeychainService.load("jwt") {
            request.addValue("Bearer \(token)", forHTTPHeaderField: "Authorization")
        }

        let (data, response) = try await URLSession.shared.data(for: request)
        try validate(response: response, data: data)

        return try JSONDecoder().decode(U.self, from: data)
    }

    private func performPost<T: Codable, U: Decodable>(_ path: String, body: T) async throws -> U {
        let url = baseURL.appendingPathComponent(path)

        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")

        if let token = KeychainService.load("jwt") {
            request.addValue("Bearer \(token)", forHTTPHeaderField: "Authorization")
        }

        request.httpBody = try JSONEncoder().encode(body)

        let (data, response) = try await URLSession.shared.data(for: request)
        try validate(response: response, data: data)

        return try JSONDecoder().decode(U.self, from: data)
    }

    private func performMultipart<U: Decodable>(
        _ path: String,
        fileName: String,
        fileData: Data
    ) async throws -> U {

        let url = baseURL.appendingPathComponent(path)

        var request = URLRequest(url: url)
        request.httpMethod = "POST"

        if let token = KeychainService.load("jwt") {
            request.addValue("Bearer \(token)", forHTTPHeaderField: "Authorization")
        }

        let boundary = "WebAppBoundary"
        request.setValue("multipart/form-data; boundary=\(boundary)", forHTTPHeaderField: "Content-Type")

        var body = Data()
        let mimeType = "image/jpeg"

        body.append("--\(boundary)\r\n")
        body.append("Content-Disposition: form-data; name=\"file\"; filename=\"\(fileName)\"\r\n")
        body.append("Content-Type: \(mimeType)\r\n\r\n")
        body.append(fileData)
        body.append("\r\n")
        body.append("--\(boundary)--\r\n")

        request.httpBody = body

        let (data, response) = try await URLSession.shared.data(for: request)
        try validate(response: response, data: data)

        return try JSONDecoder().decode(U.self, from: data)
    }

    private func performPostData<T: Encodable>(_ path: String, body: T) async throws -> (data: Data, mimeType: String?) {
        let url = baseURL.appendingPathComponent(path)

        var request = URLRequest(url: url)
        request.httpMethod = "POST"
        request.addValue("application/json", forHTTPHeaderField: "Content-Type")

        if let token = KeychainService.load("jwt") {
            request.addValue("Bearer \(token)", forHTTPHeaderField: "Authorization")
        }

        request.httpBody = try JSONEncoder().encode(body)

        let (data, response) = try await URLSession.shared.data(for: request)
        try validate(response: response, data: data)

        let mime = (response as? HTTPURLResponse)?
            .value(forHTTPHeaderField: "Content-Type")

        return (data, mime)
    }

    private func tryRefreshToken() async throws -> Bool {
        guard let refresh = KeychainService.load("refreshToken") else {
            logoutUser()
            return false
        }

        let body = RefreshRequest(token: refresh)

        do {
            let response: RefreshResponse = try await performPost("/api/auth/refresh", body: body)

            KeychainService.save(response.token, for: "jwt")
            KeychainService.save(response.refreshToken, for: "refreshToken")

            return true
        } catch {
            logoutUser()
            return false
        }
    }

    private func logoutUser() {
        KeychainService.delete(for: "jwt")
        KeychainService.delete(for: "refreshToken")
        NotificationCenter.default.post(name: .userLoggedOut, object: nil)
    }

    private func validate(response: URLResponse, data: Data) throws {
        guard let http = response as? HTTPURLResponse else {
            throw ApiError.unknown
        }

        switch http.statusCode {
        case 200...299:
            return
        case 401:
            throw ApiError.unauthorized
        default:
            let message = String(data: data, encoding: .utf8) ?? "Unknown error"
            throw ApiError.server(message: message)
        }
    }
}

extension Notification.Name {
    static let userLoggedOut = Notification.Name("userLoggedOut")
}
