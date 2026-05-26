//
//  BackupView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 29.04.2026.
//

import SwiftUI
import Photos

struct BackupView: View {
    let title: String
    @ObservedObject var viewModel: BackupViewModel
    let background: Color = Color(red: 224/255, green: 224/255, blue: 224/255)

    private let columns = [
        GridItem(.flexible()),
        GridItem(.flexible()),
        GridItem(.flexible())
    ]

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(alignment: .leading, spacing: 24) {

                    HStack {
                        Spacer()
                        Text(title)
                            .font(.largeTitle)
                            .bold()
                        Spacer()
                    }

                    if let status = viewModel.uploadStatus {
                        Text(status)
                            .foregroundColor(.black)
                            .padding(.horizontal)
                    }

                    if viewModel.isLoading && viewModel.thumbnails.isEmpty {
                        ProgressView("Načítám fotky…")
                            .padding()
                    }

                    LazyVGrid(columns: columns, spacing: 4) {
                        ForEach(Array(viewModel.thumbnails.enumerated()), id: \.offset) { index, image in

                            ZStack {
                                Image(uiImage: image)
                                    .resizable()
                                    .scaledToFill()
                                    .frame(height: 120)
                                    .clipped()

                                if let asset = viewModel.assetForThumbnail(index: index),
                                   asset.localIdentifier == viewModel.uploadingAssetID {

                                    Color.black.opacity(0.4)
                                        .frame(height: 120)

                                    ProgressView()
                                        .progressViewStyle(CircularProgressViewStyle(tint: .white))
                                        .scaleEffect(1.5)
                                }
                            }
                            .onAppear {
                                if index == viewModel.thumbnails.count - 10 {
                                    viewModel.loadNextBatch()
                                }
                            }
                        }
                    }
                    .padding(.horizontal, 4)

                    if viewModel.isLoading {
                        ProgressView()
                            .padding()
                    }
                }
            }
            .background(background.ignoresSafeArea())
            .onAppear {
                viewModel.restartIfNeeded()
            }
        }
    }
}
