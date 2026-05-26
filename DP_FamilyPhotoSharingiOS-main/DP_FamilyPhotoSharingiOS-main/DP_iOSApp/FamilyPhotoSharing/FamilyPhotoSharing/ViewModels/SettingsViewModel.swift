//
//  SettingsViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 07.05.2026.
//

import Foundation
import Combine

@MainActor
final class SettingsViewModel: ObservableObject {
    @Published var username: String = ""
    @Published var familyName: String = ""

    @Published var backupEnabled: Bool
    @Published var backupFamilyMode: Bool
    @Published var didSave: Bool = false

    private let service = SettingsService()
    private let userService: UserService?

    init(userService: UserService?) {
        self.userService = userService

        // 1) načtení cache
        self.username = service.userName
        self.familyName = service.familyName

        self.backupEnabled = service.backupEnabled
        self.backupFamilyMode = service.backupFamilyMode

        // 2) pokus o načtení API
        Task {
            await loadFromApi()
        }
    }

    func loadFromApi() async {
        guard let userService else { return }

        do {
            let info = try await userService.loadUserInfo()

            self.username = info.name + " " + info.surname
            self.familyName = info.groupName

            service.userName = info.name + " " + info.surname
            service.familyName = info.groupName

        } catch {
            print("Nepodařilo se načíst user info z API, používám cache.")
        }
    }

    func save() {
        service.backupEnabled = backupEnabled
        service.backupFamilyMode = backupFamilyMode
        didSave = true
    }
    
    func logout() {
        KeychainService.delete(for: "jwt")
        KeychainService.delete(for: "refreshToken")
        service.login = ""
        service.backupEnabled = false
    }
}
