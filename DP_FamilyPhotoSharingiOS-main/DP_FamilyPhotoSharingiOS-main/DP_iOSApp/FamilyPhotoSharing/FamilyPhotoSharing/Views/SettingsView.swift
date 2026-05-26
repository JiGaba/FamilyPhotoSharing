//
//  SettingsView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 07.05.2026.
//

import SwiftUI

struct SettingsView: View {
    let title: String
    let onLogout: () -> Void
    @StateObject var vm: SettingsViewModel

    @State private var showLogoutAlert = false

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(alignment: .leading, spacing: 28) {

                    HStack {
                        Text(title)
                            .font(.largeTitle)
                            .bold()

                        Spacer()

                        Button {
                            showLogoutAlert = true
                        } label: {
                            Text("Odhlásit")
                                .foregroundColor(.red)
                                .font(.system(size: 18, weight: .semibold))
                        }
                    }
                    .padding(.horizontal)

                    VStack(alignment: .leading, spacing: 16) {

                        VStack(alignment: .leading, spacing: 4) {
                            Text("Uživatelské jméno")
                                .font(.headline)
                            Text(vm.username)
                                .font(.system(size: 18))
                                .padding()
                                .background(Color(red: 224/255, green: 224/255, blue: 224/255).opacity(0.3))
                                .cornerRadius(12)
                        }

                        VStack(alignment: .leading, spacing: 4) {
                            Text("Rodina")
                                .font(.headline)
                            Text(vm.familyName)
                                .font(.system(size: 18))
                                .padding()
                                .background(Color(red: 224/255, green: 224/255, blue: 224/255).opacity(0.3))
                                .cornerRadius(12)
                        }
                    }
                    .padding(.horizontal)

                    VStack(alignment: .leading, spacing: 20) {

                        Toggle("Zálohování zapnuto", isOn: $vm.backupEnabled)
                            .font(.system(size: 18))
                            .padding()
                            .background(Color(red: 224/255, green: 224/255, blue: 224/255).opacity(0.3))
                            .cornerRadius(12)

                        Toggle("Zálohovat do rodinného alba", isOn: $vm.backupFamilyMode)
                            .font(.system(size: 18))
                            .padding()
                            .background(Color(red: 224/255, green: 224/255, blue: 224/255).opacity(0.3))
                            .cornerRadius(12)
                    }
                    .padding(.horizontal)

                    Button {
                        vm.save()
                        hideKeyboard()
                    } label: {
                        Text("Uložit nastavení")
                            .frame(maxWidth: .infinity)
                            .padding()
                            .background(Color.blue)
                            .foregroundColor(.white)
                            .cornerRadius(12)
                            .font(.system(size: 20, weight: .semibold))
                    }
                    .padding(.horizontal)
                    .alert("Nastavení bylo uloženo", isPresented: $vm.didSave) {
                        Button("OK", role: .cancel) {}
                    }
                }
                .padding(.vertical, 20)
            }
            .background(Color(red: 224/255, green: 224/255, blue: 224/255).ignoresSafeArea())
            .onTapGesture { hideKeyboard() }
            .alert("Opravdu se chceš odhlásit?", isPresented: $showLogoutAlert) {
                Button("Zrušit", role: .cancel) {}
                Button("Odhlásit", role: .destructive) {
                    vm.logout()
                    onLogout()
                }
            }
        }
    }
}
