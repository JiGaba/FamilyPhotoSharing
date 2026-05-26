//
//  PersonalGalleryView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 29.04.2026.
//

import SwiftUI
struct PersonalGalleryView: View {
    @StateObject private var viewModel: PersonalGalleryViewModel

    let title: String
    
    let photoService: PhotoService
    let photoRepository: PhotoRepository

    private let mainAlbum = AlbumTemp(
        id: -1,
        title: "Hlavní soukromá galerie",
        kind: .personal
    )

    init(title: String, service: AlbumService, photoService: PhotoService) {
        let repository = AlbumRepository(api: service)
        _viewModel = StateObject(wrappedValue: PersonalGalleryViewModel(repository: repository))
        self.title = title
        self.photoService = photoService
        self.photoRepository = PhotoRepository(api: photoService)
    }

    let columns = [
        GridItem(.flexible()),
        GridItem(.flexible()),
        GridItem(.flexible())
    ]

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(alignment: .leading, spacing: 20) {

                    HStack {
                        Spacer()
                        Text(title)
                            .font(.largeTitle)
                            .bold()
                        Spacer()
                    }

                    VStack(alignment: .leading) {
                        Text("Hlavní album")
                            .font(.headline)
                            .padding(.horizontal)

                        Button {
                            viewModel.openMainAlbum(mainAlbum)
                        } label: {
                            AlbumSquare(album: mainAlbum, photoService: photoService)
                                .frame(height: 200)
                        }
                        .buttonStyle(.plain)
                        .padding(.horizontal)
                    }

                    VStack(alignment: .leading) {
                        Text("Soukromá alba")
                            .font(.headline)
                            .padding(.horizontal)

                        ZStack {
                            LazyVGrid(columns: columns, spacing: 16) {
                                ForEach(viewModel.privateAlbums) { album in
                                    Button {
                                        viewModel.openPrivateAlbum(album)
                                    } label: {
                                        AlbumSquare(
                                            album: AlbumTemp(
                                                id: album.id,
                                                title: album.albumName,
                                                kind: .personal,
                                            ),
                                            photoService: photoService
                                        )
                                        .frame(height: 120)
                                    }
                                    .buttonStyle(.plain)
                                }
                            }
                            .padding(.horizontal)

                            if viewModel.isLoading {
                                ProgressView()
                                    .scaleEffect(1.3)
                                    .padding(20)
                                    .background(Color.black.opacity(0.1))
                                    .cornerRadius(12)
                            }
                        }
                    }
                }
            }
            .background(Color(red: 204/255, green: 255/255, blue: 229/255).ignoresSafeArea())
            .task {
                AlbumTitleCache.shared.photoService = photoService
                await viewModel.load()
            }
            .navigationDestination(item: $viewModel.selectedDetail) { detail in
                GalleryDetailView(
                    viewModel: GalleryDetailViewModel(album: detail, repository: photoRepository, service: photoService)
                )
            }
        }
    }
}
