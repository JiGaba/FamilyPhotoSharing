//
//  FamilyGalleryViewModel.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

import Combine

@MainActor
final class FamilyGalleryViewModel: ObservableObject {
    @Published var familyAlbums: [Album] = []
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
            from: CacheFile.familyAlbums.filename()
        ) {
            self.familyAlbums = cached

            for album in cached {
                AlbumTitleCache.shared.set(
                    albumId: album.id,
                    kind: .family,
                    titlePhotoId: album.titlePhotoId
                )
            }
        }

        // 2) API
        isLoading = true
        let fresh = await repository.getFamilyAlbums()
        self.familyAlbums = fresh
        isLoading = false

        for album in fresh {
            AlbumTitleCache.shared.set(
                albumId: album.id,
                kind: .family,
                titlePhotoId: album.titlePhotoId
            )
        }
    }

    func openMainAlbum(_ album: AlbumTemp) {
        selectedDetail = AlbumDetailModel(
            id: album.id,
            title: album.title,
            type: ThumbnailData.mainGroupGallery
        )
    }

    func openFamilyAlbum(_ album: Album) {
        selectedDetail = AlbumDetailModel(
            id: album.id,
            title: album.albumName,
            type: ThumbnailData.groupAlbum
        )
    }
}
