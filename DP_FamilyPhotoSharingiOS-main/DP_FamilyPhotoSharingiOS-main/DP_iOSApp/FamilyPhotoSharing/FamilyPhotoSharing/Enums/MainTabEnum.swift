//
//  MainTabEnum.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 29.04.2026.
//

enum MainTab: Int, CaseIterable {
    case personalGallery, familyGallery, backup, sharedGallery, profile
    
    var title: String {
        switch self {
        case .personalGallery: return "Soukromá alba"
        case .familyGallery: return "Rodinná alba"
        case .backup: return "Zálohování"
        case .sharedGallery: return "Sdílená alba"
        case .profile: return "Profil"
        }
    }
    
    var systemImage: String {
        switch self {
        case .personalGallery: return "photo.on.rectangle.angled"
        case .familyGallery: return "person.3.fill"
        case .backup: return "externaldrive.fill"
        case .sharedGallery: return "link"
        case .profile: return "person.crop.circle.badge.checkmark"
        }
    }
}
