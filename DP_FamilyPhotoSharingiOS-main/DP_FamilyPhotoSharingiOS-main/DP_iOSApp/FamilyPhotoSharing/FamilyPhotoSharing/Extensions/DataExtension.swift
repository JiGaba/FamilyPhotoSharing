//
//  DataExtension.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 24.04.2026.
//

import Foundation   

extension Data {
    mutating func append(_ string: String) {
        if let data = string.data(using: .utf8) {
            append(data)
        }
    }
}
