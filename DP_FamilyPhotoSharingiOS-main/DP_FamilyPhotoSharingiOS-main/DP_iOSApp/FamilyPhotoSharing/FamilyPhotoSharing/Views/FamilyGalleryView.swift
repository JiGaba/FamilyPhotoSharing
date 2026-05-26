//
//  FamilyGalleryView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 29.04.2026.
//

import SwiftUI

struct FamilyGalleryView: View {
    @StateObject private var viewModel: FamilyGalleryViewModel
    
    let photoService: PhotoService
    let photoRepository: PhotoRepository
    let title: String

    private let mainAlbum = AlbumTemp(id: 0, title: "Rodinné album", kind: .family)
    
    init(title: String, service: AlbumService, photoService: PhotoService) {
        let repository = AlbumRepository(api: service)
        _viewModel = StateObject(wrappedValue: FamilyGalleryViewModel(repository: repository))
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
                            .foregroundColor(.black)
                        Spacer()
                    }
                    
                    VStack(alignment: .leading) {
                        Text("Hlavní album")
                            .font(.headline)
                            .padding(.horizontal)
                            .foregroundColor(.black)
                        
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
                        Text("Rodinná alba")
                            .font(.headline)
                            .padding(.horizontal)
                        
                        ZStack {
                            LazyVGrid(columns: columns, spacing: 16) {
                                ForEach(viewModel.familyAlbums) { album in
                                    Button {
                                        viewModel.openFamilyAlbum(album)
                                    } label: {
                                        AlbumSquare(
                                            album: AlbumTemp(
                                                id: 0,
                                                title: album.albumName,
                                                kind: .family
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
            .background(Color(red: 204/255, green: 255/255, blue: 255/255).ignoresSafeArea())
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

