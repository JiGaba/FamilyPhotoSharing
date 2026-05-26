//
//  GalleryPhotoModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//

import Foundation

struct GalleryPhoto: Identifiable, Hashable, Equatable {
    let id: Int
    var thumbnail: Data? = nil
}
