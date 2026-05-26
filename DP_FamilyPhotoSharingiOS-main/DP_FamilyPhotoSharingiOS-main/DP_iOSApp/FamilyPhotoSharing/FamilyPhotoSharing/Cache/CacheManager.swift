//
//  CacheManager.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 01.05.2026.
//

import Foundation

final class CacheManager {
    static let shared = CacheManager()
    private init() {}

    private var cacheURL: URL {
        let url = FileManager.default.urls(for: .cachesDirectory, in: .userDomainMask)[0]
        //print("Cache directory:", url.path)
        return url
    }

    func save<T: Encodable>(_ object: T, as filename: String) {
        let url = cacheURL.appendingPathComponent(filename)
        //print("Saving to:", url.path)

        do {
            let data = try JSONEncoder().encode(object)
            try data.write(to: url)
            //print("Saved OK:", filename)
        } catch {
            //print("Cache save error:", error)
        }
    }

    func load<T: Decodable>(_ type: T.Type, from filename: String) -> T? {
        let url = cacheURL.appendingPathComponent(filename)
        //print("Loading from:", url.path)

        do {
            let data = try Data(contentsOf: url)
            let decoded = try JSONDecoder().decode(type, from: data)
            //print("Loaded OK:", filename)
            return decoded
        } catch {
            //print("Cache load error:", error)
            return nil
        }
    }
}
