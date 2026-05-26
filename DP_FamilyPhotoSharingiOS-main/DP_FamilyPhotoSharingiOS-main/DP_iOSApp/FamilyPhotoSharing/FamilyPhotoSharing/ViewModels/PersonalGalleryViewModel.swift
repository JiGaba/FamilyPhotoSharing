//
//  PersonalGalleryViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

import Combine
import Foundation

@MainActor
final class PersonalGalleryViewModel: ObservableObject {
    @Published var privateAlbums: [Album] = []
    @Published var isLoading = false
    @Published var selectedDetail: AlbumDetailModel?

    private let repository: AlbumRepositoryProtocol

    init(repository: AlbumRepositoryProtocol) {
        self.repository = repository
    }

    func load() async {
        
        // 1) CACHE FIRST
        if let cached: [Album] = CacheManager.shared.load(
            [Album].self,
            from: CacheFile.privateAlbums.filename()
        ) {
            self.privateAlbums = cached

            for album in cached {
                AlbumTitleCache.shared.set(
                    albumId: album.id,
                    kind: .personal,
                    titlePhotoId: album.titlePhotoId
                )
            }
        }

        // 2) API
        isLoading = true

        do {
            let fresh = try await withTimeout(seconds: 5) {
                await self.repository.getPrivateAlbums()
            }

            self.privateAlbums = fresh

            for album in fresh {
                AlbumTitleCache.shared.set(
                    albumId: album.id,
                    kind: .personal,
                    titlePhotoId: album.titlePhotoId
                )
            }

        } catch {
            print("API timeout nebo chyba: \(error.localizedDescription)")
        }

        isLoading = false
    }


    func openMainAlbum(_ album: AlbumTemp) {
        selectedDetail = AlbumDetailModel(
            id: album.id,
            title: album.title,
            type: ThumbnailData.mainPersonalGallery
        )
    }

    func openPrivateAlbum(_ album: Album) {
        selectedDetail = AlbumDetailModel(
            id: album.id,
            title: album.albumName,
            type: ThumbnailData.album
        )
    }
}
