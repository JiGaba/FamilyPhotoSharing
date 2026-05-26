//
//  TimeoutHelper.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 07.05.2026.
//

import Foundation

func withTimeout<T>(seconds: Double, operation: @escaping () async throws -> T) async throws -> T {
    try await withThrowingTaskGroup(of: T.self) { group in
        
        group.addTask {
            try await operation()
        }
        
        group.addTask {
            try await Task.sleep(nanoseconds: UInt64(seconds * 1_000_000_000))
            throw URLError(.timedOut)
        }
        
        let result = try await group.next()!
        group.cancelAll()
        return result
    }
}
