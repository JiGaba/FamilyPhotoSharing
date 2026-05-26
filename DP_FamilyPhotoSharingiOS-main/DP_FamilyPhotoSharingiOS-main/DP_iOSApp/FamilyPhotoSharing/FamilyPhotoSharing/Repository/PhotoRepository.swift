//
//  PhotoRepository.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//

protocol PhotoRepositoryProtocol {
    func loadInitialGallery(album: AlbumDetailModel) async -> PhotoResponseData?
    func loadPhotosPage(album: AlbumDetailModel, page: Int, part: Int) async -> [PhotoItem]
}

final class PhotoRepository: PhotoRepositoryProtocol {
    private let api: PhotoService

    init(api: PhotoService) {
        self.api = api
    }

    func loadInitialGallery(album: AlbumDetailModel) async -> PhotoResponseData? {

        let cacheKey = CacheFile.galleryMetadata.filename(
            id: album.id,
            albumType: album.type
        )

        // 1) load cache
        let cached: PhotoResponseData? = CacheManager.shared.load(
            PhotoResponseData.self,
            from: cacheKey
        )

        // 2) try API
        do {
            let response: PhotoResponseData

            switch album.type {
            case .mainPersonalGallery:
                response = try await api.loadMainPersonalAlbum(request: MainGalleryRequest(page: 1))

            case .mainGroupGallery:
                response = try await api.loadMainGroupAlbum(request: MainGalleryRequest(page: 1))

            case .mainGroupGalleryOwnPhoto:
                response = try await api.loadMainGroupAlbumOwnPhoto(request: MainGalleryRequest(page: 1))

            case .album:
                response = try await api.showAlbum(request: GalleryRequest(page: 1, albumId: album.id))

            case .groupAlbum:
                response = try await api.showGroupAlbum(request: GalleryRequest(page: 1, albumId: album.id))

            case .mySharedAlbum:
                response = try await api.showMySharedAlbum(request: GalleryRequest(page: 1, albumId: album.id))

            case .sharedAlbum:
                response = try await api.showSharedAlbum(request: GalleryRequest(page: 1, albumId: album.id))
            }

            CacheManager.shared.save(response, as: cacheKey)
            return response

        } catch {
            return cached
        }
    }

    func loadPhotosPage(album: AlbumDetailModel, page: Int, part: Int) async -> [PhotoItem] {

        let cacheKey = CacheFile.galleryPart.filename(
            id: album.id,
            page: page,
            part: part,
            albumType: album.type
        )

        // 1) load cache
        let cached: [PhotoItem]? = CacheManager.shared.load([PhotoItem].self, from: cacheKey)

        // 2) try API
        do {
            let request = LoadPhotosRequest(
                thumbnailData: album.type.rawValue,
                page: page,
                part: part,
                albumId: album.id
            )

            let response = try await api.loadPhotos(request: request)

            CacheManager.shared.save(response.data, as: cacheKey)

            return response.data

        } catch {
            return cached ?? []
        }
    }
}
