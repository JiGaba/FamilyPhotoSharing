//
//  SettingsService.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 23.04.2026.
//

import Foundation

class SettingsService {
    private let defaults = UserDefaults.standard
    
    var url: String {
        get { defaults.string(forKey: "url") ?? "" }
        set { defaults.set(newValue, forKey: "url") }
    }
    
    var login: String {
        get { defaults.string(forKey: "login") ?? "" }
        set { defaults.set(newValue, forKey: "login") }
    }
    
    var userName: String {
        get { defaults.string(forKey: "userName") ?? "uživatel" }
        set { defaults.set(newValue, forKey: "userName") }
    }
    
    var familyName: String {
        get { defaults.string(forKey: "familyName") ?? "Moje rodina" }
        set { defaults.set(newValue, forKey: "familyName") }
    }

    // zálohování zapnuto/vypnuto
    var backupEnabled: Bool {
        get { defaults.bool(forKey: "backupEnabled") }
        set { defaults.set(newValue, forKey: "backupEnabled") }
    }

    // typ zálohy (false = soukromé, true = rodinné)
    var backupFamilyMode: Bool {
        get { defaults.bool(forKey: "backupFamilyMode") }
        set { defaults.set(newValue, forKey: "backupFamilyMode") }
    }
    
    var uploadedPhotoIDs: [String] {
        get { defaults.stringArray(forKey: "uploadedPhotoIDs") ?? [] }
        set { defaults.set(newValue, forKey: "uploadedPhotoIDs") }
    }
    
    var failedPhotoIDs: [String] {
        get { defaults.stringArray(forKey: "failedPhotoIDs") ?? [] }
        set { defaults.set(newValue, forKey: "failedPhotoIDs") }
    }
}
