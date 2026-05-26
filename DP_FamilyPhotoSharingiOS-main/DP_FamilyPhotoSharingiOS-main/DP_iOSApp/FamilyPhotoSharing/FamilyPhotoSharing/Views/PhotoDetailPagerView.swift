//
//  PhotoDetailPagerView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 05.05.2026.
//

import SwiftUI

struct PhotoDetailPagerView: View {
    let photos: [GalleryPhoto]
    let startIndex: Int
    let service: PhotoService

    @State private var index: Int

    init(photos: [GalleryPhoto], startIndex: Int, service: PhotoService) {
        self.photos = photos
        self.startIndex = startIndex
        self.service = service
        _index = State(initialValue: startIndex)
    }

    var body: some View {
        TabView(selection: $index) {
            ForEach(photos.indices, id: \.self) { i in
                PhotoDetailView(
                    viewModel: PhotoDetailViewModel(
                        photoId: photos[i].id,
                        service: service
                    )
                )
                .tag(i)
            }
        }
        .tabViewStyle(.page)
        .indexViewStyle(.page(backgroundDisplayMode: .always))
        .navigationBarTitleDisplayMode(.inline)
    }
}
