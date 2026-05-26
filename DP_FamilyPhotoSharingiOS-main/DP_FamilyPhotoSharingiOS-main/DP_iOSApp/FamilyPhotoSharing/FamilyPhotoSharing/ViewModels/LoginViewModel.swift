//
//  LoginViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

import Foundation
import Combine
import SwiftUI

@MainActor
class LoginViewModel: ObservableObject {
    @Published var url: String
    @Published var login: String
    @Published var password: String = ""
    @Published var jwtToken: String?
    @Published var errorMessage: String?
    
    private let settings = SettingsService()
    
    init() {
        self.url = settings.url
        self.login = settings.login
    }

    func sendToAPI() async -> Bool {
        jwtToken = nil
        errorMessage = nil
        
        guard let baseURL = URL(string: url) else {
            errorMessage = "Neplatná URL"
            return false
        }

        let api = LoginApiClient(baseURL: baseURL)
        
        do {
            let response = try await api.login(login: login, password: password)
            jwtToken = response.token
            
            // uložit JWT
            KeychainService.save(response.token, for: "jwt")
            KeychainService.save(response.refreshToken.token, for: "refreshToken")

            // uložit URL + login
            settings.url = url
            settings.login = login

            return true

        } catch {
            errorMessage = "Chyba: \(error.localizedDescription)"
            return false
        }
    }
}
