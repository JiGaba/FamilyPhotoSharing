//
//  SharedGalleryView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 29.04.2026.
//

import SwiftUI

struct SharedGalleryView: View {
    @StateObject private var viewModel: SharedGalleryViewModel
    
    let title: String
    let photoService: PhotoService
    let photoRepository: PhotoRepository
    
    init(title: String, service: SharedAlbumService, photoService: PhotoService) {
        let repository = SharedAlbumRepository(api: service)
        _viewModel = StateObject(wrappedValue: SharedGalleryViewModel(repository: repository))
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
                        Text("Vlastní sdílená alba")
                            .font(.headline)
                            .padding(.horizontal)
                        
                        ZStack {
                            LazyVGrid(columns: columns, spacing: 16) {
                                ForEach(viewModel.sharedAlbums) { album in
                                    Button {
                                        viewModel.openSharedAlbum(album)
                                    } label: {
                                        AlbumSquare(
                                            album: AlbumTemp(
                                                id: album.id,
                                                title: album.albumName,
                                                kind: .shared,
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
                    
                    VStack(alignment: .leading) {
                        Text("Sdílená alba")
                            .font(.headline)
                            .padding(.horizontal)
                        
                        ZStack {
                            LazyVGrid(columns: columns, spacing: 16) {
                                ForEach(viewModel.sharedAlbumsAll) { album in
                                    Button {
                                        viewModel.openSharedAlbum(album)
                                    } label: {
                                        AlbumSquare(
                                            album: AlbumTemp(
                                                id: album.id,
                                                title: album.albumName,
                                                kind: .shared
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
            .background(Color(red: 224/255, green: 224/255, blue: 204/255).ignoresSafeArea())
            .task {
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
