# Base image (AMD64)
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libxcb1 \
    libuuid1 \
    uuid-runtime \
    --no-install-recommends && \
    rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV FILE_MAX_SIZE=52428800

# Build stage (AMD64)
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["FamilyPhotoSharing/FamilyPhotoSharing.csproj", "FamilyPhotoSharing/"]
COPY ["ModelLayer/ModelLayer.csproj", "ModelLayer/"]
COPY ["ServiceLayer/ServiceLayer.csproj", "ServiceLayer/"]
COPY ["DataAccessLayer/DataAccessLayer.csproj", "DataAccessLayer/"]
COPY ["EncryptionLayer/EncryptionLayer.csproj", "EncryptionLayer/"]
COPY ["FileAccessLayer/FileAccessLayer.csproj", "FileAccessLayer/"]

RUN dotnet restore "./FamilyPhotoSharing/FamilyPhotoSharing.csproj"

COPY . .
WORKDIR "/src/FamilyPhotoSharing"
RUN dotnet build "./FamilyPhotoSharing.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage (AMD64)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FamilyPhotoSharing.csproj" \
    -c $BUILD_CONFIGURATION \
    -r linux-x64 \
    --self-contained false \
    -o /app/publish \
    /p:UseAppHost=false

# Final image (AMD64)
FROM --platform=linux/amd64 base AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FamilyPhotoSharing.dll"]
