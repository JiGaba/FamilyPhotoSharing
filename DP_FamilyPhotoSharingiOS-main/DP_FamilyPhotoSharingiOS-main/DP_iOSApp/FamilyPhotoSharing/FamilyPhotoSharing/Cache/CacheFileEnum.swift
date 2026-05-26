//
//  CacheFileEnum.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

enum CacheFile: String {
    case privateAlbums = "albums_private.json"
    case familyAlbums = "albums_family.json"
    case sharedAlbums = "albums_shared.json"
    case sharedAlbumsAll = "albums_shared_all.json"
    case albumDetail = "album_detail_"
    case familyAlbumDetail = "family_album_detail_"
    case sharedAlbumDetail = "shared_album_detail_"
    case galleryMetadata = "gallery_metadata_"
    case galleryPart = "gallery_part_"
    case thumbnail = "thumb_"

    nonisolated func filename(
        id: Int? = nil,
        page: Int? = nil,
        part: Int? = nil,
        albumType: ThumbnailData? = nil
    ) -> String {

        let prefix = albumType?.rawValue ?? -1

        switch self {

        case .privateAlbums,
             .familyAlbums,
             .sharedAlbums,
             .sharedAlbumsAll:
            return rawValue

        case .galleryMetadata:
            guard let id else { fatalError("ID je nutné pro galleryMetadata.") }
            return "\(rawValue)\(prefix)_\(id).json"

        case .galleryPart:
            guard let id, let page, let part else {
                fatalError("ID, page a part jsou povinné pro galleryPart.")
            }
            return "\(rawValue)\(prefix)_\(id)_p\(page)_pt\(part).json"

        case .thumbnail:
            guard let id else { fatalError("ID je nutné pro thumbnail.") }
            return "\(rawValue)\(id).bin"

        case .albumDetail,
             .familyAlbumDetail,
             .sharedAlbumDetail:
            guard let id else { fatalError("ID je nutné pro detail alba.") }
            return "\(rawValue)\(id).json"
        }
    }
}
