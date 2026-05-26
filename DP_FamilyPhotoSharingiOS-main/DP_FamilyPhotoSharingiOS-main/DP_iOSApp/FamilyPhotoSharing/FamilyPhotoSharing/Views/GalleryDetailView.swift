//  GalleryDetail.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//

import SwiftUI

struct GalleryDetailView: View {
    @StateObject var viewModel: GalleryDetailViewModel

    private let columns = [
        GridItem(.flexible()),
        GridItem(.flexible()),
        GridItem(.flexible())
    ]

    var body: some View {
        ScrollView {
            LazyVGrid(columns: columns, spacing: 8) {
                ForEach(Array(viewModel.galleryPhotos.enumerated()), id: \.element.id) { index, photo in

                    VStack {
                        if let data = photo.thumbnail,
                           let uiImage = UIImage(data: data) {
                            Image(uiImage: uiImage)
                                .resizable()
                                .scaledToFill()
                                .frame(height: 120)
                                .clipped()
                        } else {
                            Rectangle()
                                .fill(Color.gray.opacity(0.3))
                                .frame(height: 120)
                                .overlay(ProgressView())
                        }
                    }
                    .onTapGesture {
                        viewModel.openPhoto(photo)
                    }
                    .onAppear {
                        if index >= viewModel.galleryPhotos.count - 6 && !viewModel.isLoadingMore {
                            Task { await viewModel.loadMore() }
                        }
                    }

                }
            }
            .padding(.horizontal)

            if viewModel.isLoadingMore {
                ProgressView("Načítám další fotky…")
                    .padding()
            }
        }
        .task {
            await viewModel.load()
        }
        .navigationTitle(viewModel.album.title)
        .navigationBarTitleDisplayMode(.inline)
        .navigationDestination(item: $viewModel.selectedPhoto) { photo in
            PhotoDetailPagerView(
                photos: viewModel.galleryPhotos,
                startIndex: viewModel.galleryPhotos.firstIndex(where: { $0.id == photo.id }) ?? 0,
                service: viewModel.service
            )
        }
    }
}
