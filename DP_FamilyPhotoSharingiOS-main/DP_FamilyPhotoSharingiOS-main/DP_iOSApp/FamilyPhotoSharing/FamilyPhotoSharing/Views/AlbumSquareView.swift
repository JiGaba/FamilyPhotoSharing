//
//  AlbumSquaereView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 29.04.2026.
//

import SwiftUI

final class AlbumTitleCache {
    static let shared = AlbumTitleCache()

    private var map: [String: Int] = [:] 
    var photoService: PhotoService!

    func set(albumId: Int, kind: AlbumKind, titlePhotoId: Int) {
        let key = cacheKey(albumId: albumId, kind: kind)
        map[key] = titlePhotoId
    }

    func titlePhotoId(for albumId: Int, kind: AlbumKind) -> Int? {
        let key = cacheKey(albumId: albumId, kind: kind)
        return map[key]
    }

    private func cacheKey(albumId: Int, kind: AlbumKind) -> String {
        switch kind {
        case .personal: return "personal_\(albumId)"
        case .shared:   return "shared_\(albumId)"
        case .family:   return "family_\(albumId)"
        }
    }
}

struct AlbumSquare: View {
    let album: AlbumTemp
    let photoService: PhotoService
    @State private var thumbnail: UIImage? = nil

    var body: some View {
        ZStack {
            if let thumbnail {
                Image(uiImage: thumbnail)
                    .resizable()
                    .scaledToFill()
                    .clipped()

            } else if let data = album.image, let img = UIImage(data: data) {
                Image(uiImage: img)
                    .resizable()
                    .scaledToFill()
                    .clipped()

            } else {
                ZStack {
                    Color.gray.opacity(0.25)
                    Image(systemName: "photo")
                        .resizable()
                        .scaledToFit()
                        .foregroundColor(.gray.opacity(0.6))
                        .padding(20)
                }
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .clipShape(RoundedRectangle(cornerRadius: 12))
        .task {
            await loadThumbnail()
        }
    }

    @MainActor
    private func loadThumbnail() async {
        guard album.id != -1 else { return }

        guard let titleId = AlbumTitleCache.shared.titlePhotoId(
            for: album.id,
            kind: album.kind
        ), titleId != 0 else { return }

        let cacheName = cacheKey(for: titleId)

        // 1) CACHE FIRST
        if let data: Data = CacheManager.shared.load(Data.self, from: cacheName),
           let img = UIImage(data: data) {
            self.thumbnail = img
            return
        }

        let request = GetPhotoRequest(photoId: titleId)
        do {
            let (data, _) = try await photoService.loadThumbnail(request: request)
            CacheManager.shared.save(data, as: cacheName)

            if let img = UIImage(data: data) {
                self.thumbnail = img
            }
        } catch {
        }
    }

    private func cacheKey(for photoId: Int) -> String {
        switch album.kind {
        case .personal:
            return "personal_\(photoId)"
        case .shared:
            return "shared_\(photoId)"
        case .family:
            return "family_\(photoId)"
        }
    }
}
