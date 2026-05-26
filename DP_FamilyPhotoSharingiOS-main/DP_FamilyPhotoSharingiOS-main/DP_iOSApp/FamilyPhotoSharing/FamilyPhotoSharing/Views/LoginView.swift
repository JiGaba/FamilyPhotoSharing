
//
//  LoginView.swift
//  FamilyPhotoSharing
//
//  Created by Jiří Gába on 22.04.2026.
//

import SwiftUI

struct LoginView: View {
    let title: String
    let onLogin: () -> Void
    @StateObject private var vm: LoginViewModel
    @State private var showErrorAlert = false

    init(title: String, onLogin: @escaping () -> Void) {
        self.title = title
        self.onLogin = onLogin
        _vm = StateObject(wrappedValue: LoginViewModel())
    }

    init(title: String, vm: LoginViewModel, onLogin: @escaping () -> Void) {
        self.title = title
        self.onLogin = onLogin
        _vm = StateObject(wrappedValue: vm)
    }

    var body: some View {
        NavigationStack {
            ScrollView {
                VStack(alignment: .leading, spacing: 28) {

                    HStack {
                        Spacer()
                        Text(title)
                            .font(.largeTitle)
                            .bold()
                        Spacer()
                    }

                    VStack(alignment: .leading, spacing: 18) {

                        TextField("URL serveru", text: $vm.url)
                            .keyboardType(.URL)
                            .autocapitalization(.none)
                            .padding()
                            .background(Color.white.opacity(0.9))
                            .cornerRadius(12)

                        TextField("Přihlašovací jméno", text: $vm.login)
                            .autocapitalization(.none)
                            .padding()
                            .background(Color.white.opacity(0.9))
                            .cornerRadius(12)

                        SecureField("Heslo", text: $vm.password)
                            .padding()
                            .background(Color.white.opacity(0.9))
                            .cornerRadius(12)
                    }
                    .padding(.horizontal)

                    Button {
                        Task {
                            let success = await vm.sendToAPI()
                            hideKeyboard()

                            if success {
                                onLogin()
                            } else {
                                showErrorAlert = true
                            }
                        }
                    } label: {
                        Text("Přihlásit")
                            .frame(maxWidth: .infinity)
                            .padding()
                            .background(Color.blue)
                            .foregroundColor(.white)
                            .cornerRadius(12)
                            .font(.system(size: 20, weight: .semibold))
                    }
                    .padding(.horizontal)

                    if let token = vm.jwtToken {
                        VStack(alignment: .leading, spacing: 8) {
                            Text("JWT Token")
                                .font(.headline)
                            Text(token)
                                .textSelection(.enabled)
                                .font(.footnote)
                        }
                        .padding()
                        .background(Color.white.opacity(0.9))
                        .cornerRadius(12)
                        .padding(.horizontal)
                    }
                }
            }
            .background(Color(red: 224/255, green: 224/255, blue: 224/255).ignoresSafeArea())
            .onTapGesture { hideKeyboard() }
            .alert("Chybné přihlašovací údaje", isPresented: $showErrorAlert) {
                Button("OK", role: .cancel) {}
            } message: {
                Text("Zkontrolujte URL, login a heslo.")
            }
        }
    }
}

