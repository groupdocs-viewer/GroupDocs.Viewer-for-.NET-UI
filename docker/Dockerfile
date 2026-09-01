# MS Core Fonts install runs in an isolated stage so cabextract / wget / dpkg
# never reach the runtime image. Only the resulting font files get copied out.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS fonts-install

RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    cabextract \
    xfonts-utils \
    ca-certificates \
    wget \
 && rm -rf /var/lib/apt/lists/*

RUN wget -q -O /tmp/mscorefonts.deb \
    https://deb.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.8.1_all.deb \
 && dpkg -i /tmp/mscorefonts.deb \
 && rm /tmp/mscorefonts.deb \
 && fc-cache -f -v


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# fontconfig is required by GroupDocs.Viewer at runtime; curl backs the HEALTHCHECK.
RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    curl \
 && rm -rf /var/lib/apt/lists/*

COPY --from=fonts-install /usr/share/fonts/truetype/msttcorefonts /usr/share/fonts/truetype/msttcorefonts
RUN fc-cache -f -v

# /app and the volume mount points must be writable by the non-root `app` user (UID 1654, defined in the base image).
RUN mkdir -p /app/Files /app/Cache /app/License \
 && chown -R app:app /app


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Nuget.config", "."]
COPY ["build/dependencies.props", "build/"]
COPY ["docker/GroupDocs.Viewer.UI.Docker/GroupDocs.Viewer.UI.Docker.csproj", "docker/GroupDocs.Viewer.UI.Docker/"]
COPY ["src/GroupDocs.Viewer.UI/GroupDocs.Viewer.UI.csproj", "src/GroupDocs.Viewer.UI/"]
COPY ["src/GroupDocs.Viewer.UI.Api/GroupDocs.Viewer.UI.Api.csproj", "src/GroupDocs.Viewer.UI.Api/"]
COPY ["src/GroupDocs.Viewer.UI.Core/GroupDocs.Viewer.UI.Core.csproj", "src/GroupDocs.Viewer.UI.Core/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Cache/GroupDocs.Viewer.UI.Api.Local.Cache.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Cache/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Storage/GroupDocs.Viewer.UI.Api.Local.Storage.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Storage/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api/GroupDocs.Viewer.UI.SelfHost.Api.csproj", "src/GroupDocs.Viewer.UI.SelfHost.Api/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api.Shared/GroupDocs.Viewer.UI.SelfHost.Api.Shared.projitems", "src/GroupDocs.Viewer.UI.SelfHost.Api.Shared/"]

RUN dotnet restore "./docker/GroupDocs.Viewer.UI.Docker/GroupDocs.Viewer.UI.Docker.csproj"

COPY . .
WORKDIR "/src/docker/GroupDocs.Viewer.UI.Docker"

RUN dotnet build "./GroupDocs.Viewer.UI.Docker.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupDocs.Viewer.UI.Docker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish --chown=app:app /app/publish .

ENV VIEWER_STORAGE_PATH=/app/Files
ENV VIEWER_CACHE_PATH=/app/Cache

VOLUME ["/app/Files", "/app/Cache", "/app/License"]

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

USER app

ENTRYPOINT ["dotnet", "GroupDocs.Viewer.UI.Docker.dll"]
