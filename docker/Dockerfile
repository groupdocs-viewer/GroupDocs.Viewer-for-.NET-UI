FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Install dependencies for Windows Fonts package
RUN apt update && apt install -y --no-install-recommends \
    fontconfig \
    cabextract \
    xfonts-utils \
    wget

# Download and install Windows Fonts package
RUN wget http://ftp.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.8.1_all.deb
RUN dpkg -i ttf-mscorefonts-installer_3.8.1_all.deb
RUN rm ttf-mscorefonts-installer_3.8.1_all.deb

# Update font cache
RUN fc-cache -f -v

# Cleanup temp files
RUN apt clean && rm -rf /var/lib/apt/lists/*

# Create default directories
RUN mkdir -p /app/Files /app/Cache /app/License

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project and dependency files
COPY ["Directory.Build.props", "."]
COPY ["Nuget.config", "."]
COPY ["build/dependencies.props", "build/"]
COPY ["docker/GroupDocs.Viewer.UI.Docker/GroupDocs.Viewer.UI.Docker.csproj", "docker/GroupDocs.Viewer.UI.Docker/"]
COPY ["src/GroupDocs.Viewer.UI/GroupDocs.Viewer.UI.csproj", "src/GroupDocs.Viewer.UI/"]
COPY ["src/GroupDocs.Viewer.UI.Api/GroupDocs.Viewer.UI.Api.csproj", "src/GroupDocs.Viewer.UI.Api/"]
COPY ["src/GroupDocs.Viewer.UI.Core/GroupDocs.Viewer.UI.Core.csproj", "src/GroupDocs.Viewer.UI.Core/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Cache/GroupDocs.Viewer.UI.Api.Local.Cache.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Cache/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Storage/GroupDocs.Viewer.UI.Api.Local.Storage.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Storage/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform.csproj", "src/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform/"]

# Restore dependencies
RUN dotnet restore "./docker/GroupDocs.Viewer.UI.Docker/GroupDocs.Viewer.UI.Docker.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/docker/GroupDocs.Viewer.UI.Docker"

# Build the project
RUN dotnet build "./GroupDocs.Viewer.UI.Docker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupDocs.Viewer.UI.Docker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Default environment variables
ENV VIEWER_STORAGE_PATH=/app/Files
ENV VIEWER_CACHE_PATH=/app/Cache

VOLUME ["/app/Files", "/app/Cache", "/app/License"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "GroupDocs.Viewer.UI.Docker.dll"]
