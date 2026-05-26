//
//  MainView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 28.04.2026.
//

import SwiftUI

struct MainView: View {
    @State private var selectedTab: MainTab = .personalGallery
    @State private var isLoggedIn: Bool = KeychainService.load("jwt") != nil

    @State private var albumService: AlbumService?
    @State private var sharedAlbumService: SharedAlbumService?
    @State private var photoService: PhotoService?
    @State private var uploadService: UploadService?
    @State private var userService: UserService?
    @State private var testService: TestService?
    @StateObject private var backupVM = BackupViewModel()

    private let loginTitle: String = "Přihlášení"

    var body: some View {
        ZStack {
            Group {
                switch selectedTab {
                case .personalGallery:
                    if let albumService, let photoService {
                        PersonalGalleryView(
                            title: MainTab.personalGallery.title,
                            service: albumService,
                            photoService: photoService
                        )
                    } else {
                        LoginView(title: loginTitle, onLogin: handleLogin)
                    }

                case .familyGallery:
                    if let albumService, let photoService {
                        FamilyGalleryView(
                            title: MainTab.familyGallery.title,
                            service: albumService,
                            photoService: photoService
                        )
                    } else {
                        LoginView(title: loginTitle, onLogin: handleLogin)
                    }

                case .backup:
                    if uploadService != nil && testService != nil {
                        BackupView(title: MainTab.backup.title, viewModel: backupVM)
                    } else {
                        LoginView(title: loginTitle, onLogin: handleLogin)
                    }

                case .sharedGallery:
                    if let sharedAlbumService, let photoService {
                        SharedGalleryView(
                            title: MainTab.sharedGallery.title,
                            service: sharedAlbumService,
                            photoService: photoService
                        )
                    } else {
                        LoginView(title: loginTitle, onLogin: handleLogin)
                    }

                case .profile:
                    if isLoggedIn, let userService {
                        SettingsView(
                            title: MainTab.profile.title,
                            onLogout: handleLogout,
                            vm: SettingsViewModel(userService: userService)
                        )
                    } else {
                        LoginView(title: loginTitle, onLogin: handleLogin)
                    }
                }
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .ignoresSafeArea()

            bottomTabBar
        }
        .onAppear {
            loadServices()

            if let uploadService, let testService {
                backupVM.uploadService = uploadService
                backupVM.testService = testService
                backupVM.loadPhotos()
                backupVM.startAutoUploadLoop()
            }
        }
        .onReceive(NotificationCenter.default.publisher(for: .userLoggedOut)) { _ in
            handleLogout()
        }
    }

    private func loadServices() {
        let urlString = SettingsService().url

        guard let url = URL(string: urlString), !urlString.isEmpty else {
            albumService = nil
            sharedAlbumService = nil
            photoService = nil
            uploadService = nil
            userService = nil
            testService = nil
            return
        }

        let albumApi = AlbumApiClient(baseURL: url)
        let sharedAlbumApi = SharedAlbumApiClient(baseURL: url)
        let photoApi = PhotoApiClient(baseURL: url)
        let uploadApi = UploadApiClient(baseURL: url)
        let userApi = UserApiClient(baseURL: url)
        let testApi = TestApiClient(baseURL: url)

        self.userService = UserService(userApi: userApi)
        self.uploadService = UploadService(uploadApi: uploadApi)
        self.photoService = PhotoService(photoApi: photoApi)
        self.albumService = AlbumService(albumApi: albumApi)
        self.sharedAlbumService = SharedAlbumService(sharedAlbumApi: sharedAlbumApi)
        self.testService = TestService(testApi: testApi)

        if let uploadService, let testService {
            backupVM.uploadService = uploadService
            backupVM.testService = testService
        }
    }

    private func handleLogin() {
        isLoggedIn = true
        loadServices()

        if let uploadService, let testService {
            backupVM.uploadService = uploadService
            backupVM.testService = testService
            backupVM.loadPhotos()
            backupVM.startAutoUploadLoop()
        }
    }

    private func handleLogout() {
        isLoggedIn = false
        selectedTab = .profile
        albumService = nil
        sharedAlbumService = nil
        photoService = nil
        uploadService = nil
        testService = nil

        backupVM.isAutoUploading = false
    }

    private var bottomTabBar: some View {
        VStack {
            Spacer()
            HStack(spacing: 32) {
                ForEach(MainTab.allCases, id: \.self) { tab in
                    Button {
                        withAnimation(.spring(response: 0.3, dampingFraction: 0.8)) {
                            selectedTab = tab
                        }
                    } label: {
                        tabButton(tab)
                    }
                    .buttonStyle(.plain)
                }
            }
            .padding(.horizontal, 20)
            .padding(.vertical, 12)
            .background(
                RoundedRectangle(cornerRadius: 26, style: .continuous)
                    .fill(.ultraThinMaterial)
                    .overlay(
                        RoundedRectangle(cornerRadius: 26, style: .continuous)
                            .strokeBorder(Color.white.opacity(0.25), lineWidth: 1)
                    )
                    .shadow(color: .black.opacity(0.25), radius: 20, x: 0, y: 10)
            )
            .padding(.horizontal, 16)
            .padding(.bottom, 12)
        }
        .ignoresSafeArea(edges: .bottom)
    }

    private func tabButton(_ tab: MainTab) -> some View {
        VStack(spacing: 4) {
            Image(systemName: tab.systemImage)
                .font(.system(size: 20, weight: .semibold))
                .symbolVariant(selectedTab == tab ? .fill : .none)
                .foregroundStyle(selectedTab == tab ? Color.accentColor : Color.secondary)

            Circle()
                .fill(selectedTab == tab ? Color.accentColor : .clear)
                .frame(width: 6, height: 6)
        }
        .frame(maxWidth: .infinity)
    }
}
