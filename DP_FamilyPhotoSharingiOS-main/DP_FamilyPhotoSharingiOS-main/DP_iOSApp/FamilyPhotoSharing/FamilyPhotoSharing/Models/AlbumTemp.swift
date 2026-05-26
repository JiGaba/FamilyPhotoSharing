//
//  AlbumTemp.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 19.05.2026.
//

import Foundation   

struct AlbumTemp: Identifiable {
    let id: Int
    let title: String
    let kind: AlbumKind
    var image: Data? = nil
}
