//
//  SharedGalleryViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

import Combine

@MainActor
final class SharedGalleryViewModel: ObservableObject {
    @Published var sharedAlbums: [SharedAlbum] = []      
    @Published var sharedAlbumsAll: [SharedAlbum] = []
    @Published var isLoading = false

    @Published var selectedDetail: AlbumDetailModel?

    private let repository: SharedAlbumRepositoryProtocol

    init(repository: SharedAlbumRepositoryProtocol) {
        self.repository = repository
    }

    func load() async {

        // 1) CACHE FIRST – moje sdílená alba
        if let cachedMine: [SharedAlbum] = CacheManager.shared.load(
            [SharedAlbum].self,
            from: CacheFile.sharedAlbums.filename()
        ) {
            self.sharedAlbums = cachedMine

            for album in cachedMine {
                AlbumTitleCache.shared.set(
                    albumId: album.id,
                    kind: .shared,
                    titlePhotoId: album.titlePhotoId
                )
            }
        }

        // 2) CACHE FIRST – všechna sdílená alba
        if let cachedAll: [SharedAlbum] = CacheManager.shared.load(
            [SharedAlbum].self,
            from: CacheFile.sharedAlbumsAll.filename()
        ) {
            self.sharedAlbumsAll = cachedAll

            for album in cachedAll {
                AlbumTitleCache.shared.set(
                    albumId: album.id,
                    kind: .shared,
                    titlePhotoId: album.titlePhotoId
                )
            }
        }

        // 3) API
        isLoading = true

        let freshMine = await repository.getMySharedAlbums()
        self.sharedAlbums = freshMine

        for album in freshMine {
            AlbumTitleCache.shared.set(
                albumId: album.id,
                kind: .shared,
                titlePhotoId: album.titlePhotoId
            )
        }

        let freshAll = await repository.getSharedAlbums()
        self.sharedAlbumsAll = freshAll

        for album in freshAll {
            AlbumTitleCache.shared.set(
                albumId: album.id,
                kind: .shared,
                titlePhotoId: album.titlePhotoId
            )
        }

        isLoading = false
    }

    func openSharedAlbum(_ album: SharedAlbum) {
        selectedDetail = AlbumDetailModel(
            id: album.id,
            title: album.albumName,
            type: ThumbnailData.sharedAlbum
        )
    }
}
