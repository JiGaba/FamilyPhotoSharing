//
//  ThumbnailDataEnum.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

enum ThumbnailData: Int, Codable {
    case mainPersonalGallery = 1
    case mainGroupGallery = 2
    case album = 3
    case groupAlbum = 4
    case mySharedAlbum = 5
    case sharedAlbum = 6
    case mainGroupGalleryOwnPhoto = 7

    var description: String {
        switch self {
        case .mainPersonalGallery:
            return "Soukromá galerie"
        case .mainGroupGallery:
            return "Rodinná galerie"
        case .album:
            return "Soukromé album"
        case .groupAlbum:
            return "Rodinné album"
        case .mySharedAlbum:
            return "Vlastní sdílené album"
        case .sharedAlbum:
            return "Sdílené album"
        case .mainGroupGalleryOwnPhoto:
            return "Rodinná galerie - vlastní foto"
        }
    }
}
