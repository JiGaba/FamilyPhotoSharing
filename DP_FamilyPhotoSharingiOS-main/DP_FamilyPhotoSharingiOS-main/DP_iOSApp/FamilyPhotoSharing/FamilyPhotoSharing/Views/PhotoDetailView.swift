//
//  PhotoDetailView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 04.05.2026.
//

import SwiftUI

struct PhotoDetailView: View {
    @StateObject var viewModel: PhotoDetailViewModel

    @State private var scale: CGFloat = 1.0
    @State private var lastScale: CGFloat = 1.0
    @State private var offset: CGSize = .zero
    @State private var lastOffset: CGSize = .zero

    var body: some View {
        GeometryReader { geo in
            ZStack {
                if let data = viewModel.fullImage,
                   let uiImage = UIImage(data: data) {

                    let image = Image(uiImage: uiImage)
                        .resizable()
                        .aspectRatio(contentMode: .fit)
                        .scaleEffect(scale)
                        .offset(offset)
                        .frame(maxWidth: .infinity, maxHeight: .infinity)
                        .padding(.bottom, geo.size.height * 0.12)
                        .gesture(magnificationGesture)
                        .onTapGesture(count: 2) {
                            withAnimation(.spring()) {
                                if scale > 1 {
                                    resetZoom()
                                } else {
                                    scale = 2.5
                                    lastScale = 2.5
                                }
                            }
                        }

                    if scale > 1 {
                        image.gesture(dragGesture)
                    } else {
                        image
                    }

                } else if viewModel.isLoading {
                    ProgressView()
                } else {
                    Text("Fotku se nepodařilo načíst.")
                }
            }
            .frame(width: geo.size.width, height: geo.size.height)
        }
        .task { await viewModel.load() }
        .background(Color.white)
        .ignoresSafeArea()
    }

    private var magnificationGesture: some Gesture {
        MagnificationGesture()
            .onChanged { value in
                scale = lastScale * value
            }
            .onEnded { _ in
                lastScale = scale
            }
    }

    private var dragGesture: some Gesture {
        DragGesture()
            .onChanged { value in
                offset = CGSize(
                    width: lastOffset.width + value.translation.width,
                    height: lastOffset.height + value.translation.height
                )
            }
            .onEnded { _ in
                lastOffset = offset
            }
    }

    private func resetZoom() {
        scale = 1
        lastScale = 1
        offset = .zero
        lastOffset = .zero
    }
}
