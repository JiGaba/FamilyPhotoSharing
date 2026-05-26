//
//  AlbumRepository.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

import Foundation

protocol AlbumRepositoryProtocol {
    func getPrivateAlbums() async -> [Album]
    func getFamilyAlbums() async -> [Album]
    func getAlbumDetail(id: Int) async -> Album?
    func getFamilyAlbumDetail(id: Int) async -> Album?
}

final class AlbumRepository: AlbumRepositoryProtocol {
    private let api: AlbumService

    init(api: AlbumService) {
        self.api = api
    }
    
    func getPrivateAlbums() async -> [Album] {
        // 1) Load cache
        let cached: [Album]? = CacheManager.shared.load(
            [Album].self,
            from: CacheFile.privateAlbums.filename()
        )

        // 2) Try API
        do {
            let fresh = try await api.loadPrivateAlbums()
            CacheManager.shared.save(fresh, as: CacheFile.privateAlbums.filename())
            return fresh
        } catch {
            return cached ?? []
        }
    }

    func getFamilyAlbums() async -> [Album] {
        let cached: [Album]? = CacheManager.shared.load(
            [Album].self,
            from: CacheFile.familyAlbums.filename()
        )

        do {
            let fresh = try await api.loadFamilyAlbums()
            CacheManager.shared.save(fresh, as: CacheFile.familyAlbums.filename())
            return fresh
        } catch {
            return cached ?? []
        }
    }

    func getAlbumDetail(id: Int) async -> Album? {
        let cached: Album? = CacheManager.shared.load(
            Album.self,
            from: CacheFile.albumDetail.filename(id: id)
        )

        do {
            let fresh = try await api.loadAlbumDetail(id: id)
            CacheManager.shared.save(fresh, as: CacheFile.albumDetail.filename(id: id))
            return fresh
        } catch {
            return cached
        }
    }

    func getFamilyAlbumDetail(id: Int) async -> Album? {
        let cached: Album? = CacheManager.shared.load(
            Album.self,
            from: CacheFile.familyAlbumDetail.filename(id: id)
        )

        do {
            let fresh = try await api.loadFamilyAlbumDetail(id: id)
            CacheManager.shared.save(fresh, as: CacheFile.familyAlbumDetail.filename(id: id))
            return fresh
        } catch {
            return cached
        }
    }
}
