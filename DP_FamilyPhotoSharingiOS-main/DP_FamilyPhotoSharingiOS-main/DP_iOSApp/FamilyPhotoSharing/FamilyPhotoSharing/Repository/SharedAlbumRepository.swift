//
//  SharedAlbumRepository.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

protocol SharedAlbumRepositoryProtocol {
    func getMySharedAlbums() async -> [SharedAlbum]
    func getSharedAlbums() async -> [SharedAlbum]
    func getSharedAlbumDetail(id: Int) async -> SharedAlbumDetail?
}

final class SharedAlbumRepository: SharedAlbumRepositoryProtocol {

    private let api: SharedAlbumService

    init(api: SharedAlbumService) {
        self.api = api
    }

    func getMySharedAlbums() async -> [SharedAlbum] {
        let cached: [SharedAlbum]? = CacheManager.shared.load(
            [SharedAlbum].self,
            from: CacheFile.sharedAlbums.filename()
        )

        do {
            let fresh = try await api.loadMySharedAlbums()
            CacheManager.shared.save(fresh, as: CacheFile.sharedAlbums.filename())
            return fresh
        } catch {
            return cached ?? []
        }
    }

    func getSharedAlbums() async -> [SharedAlbum] {
        let cached: [SharedAlbum]? = CacheManager.shared.load(
            [SharedAlbum].self,
            from: CacheFile.sharedAlbumsAll.filename()
        )

        do {
            let fresh = try await api.loadSharedAlbums()
            CacheManager.shared.save(fresh, as: CacheFile.sharedAlbumsAll.filename())
            return fresh
        } catch {
            return cached ?? []
        }
    }

    func getSharedAlbumDetail(id: Int) async -> SharedAlbumDetail? {
        let cached: SharedAlbumDetail? = CacheManager.shared.load(
            SharedAlbumDetail.self,
            from: CacheFile.sharedAlbumDetail.filename(id: id)
        )

        do {
            let fresh = try await api.loadSharedAlbumDetail(id: id)
            CacheManager.shared.save(fresh, as: CacheFile.sharedAlbumDetail.filename(id: id))
            return fresh
        } catch {
            return cached
        }
    }
}
