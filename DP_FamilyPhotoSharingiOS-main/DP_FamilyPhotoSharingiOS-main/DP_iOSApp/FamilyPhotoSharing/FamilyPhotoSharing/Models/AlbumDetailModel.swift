//
//  AlbumDetailModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//



struct AlbumDetailModel: Identifiable, Hashable {
    let id: Int
    let title: String
    let type: ThumbnailData

    init(id: Int, title: String, type: ThumbnailData) {
        self.id = id
        self.title = title
        self.type = type
    }
}

