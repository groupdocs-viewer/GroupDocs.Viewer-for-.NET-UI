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


# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# fontconfig is required by GroupDocs.Viewer at runtime
RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
 && rm -rf /var/lib/apt/lists/*

COPY --from=fonts-install /usr/share/fonts/truetype/msttcorefonts /usr/share/fonts/truetype/msttcorefonts
RUN fc-cache -f -v


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Nuget.config", "."]
COPY ["build/dependencies.props", "build/"]
COPY ["samples/GroupDocs.Viewer.UI.Sample.AzureCustomDomain/GroupDocs.Viewer.UI.Sample.AzureCustomDomain.csproj", "samples/GroupDocs.Viewer.UI.Sample.AzureCustomDomain/"]
COPY ["src/GroupDocs.Viewer.UI/GroupDocs.Viewer.UI.csproj", "src/GroupDocs.Viewer.UI/"]
COPY ["src/GroupDocs.Viewer.UI.Api/GroupDocs.Viewer.UI.Api.csproj", "src/GroupDocs.Viewer.UI.Api/"]
COPY ["src/GroupDocs.Viewer.UI.Core/GroupDocs.Viewer.UI.Core.csproj", "src/GroupDocs.Viewer.UI.Core/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Cache/GroupDocs.Viewer.UI.Api.Local.Cache.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Cache/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Storage/GroupDocs.Viewer.UI.Api.Local.Storage.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Storage/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api/GroupDocs.Viewer.UI.SelfHost.Api.csproj", "src/GroupDocs.Viewer.UI.SelfHost.Api/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api.Shared/GroupDocs.Viewer.UI.SelfHost.Api.Shared.projitems", "src/GroupDocs.Viewer.UI.SelfHost.Api.Shared/"]

RUN dotnet restore "./samples/GroupDocs.Viewer.UI.Sample.AzureCustomDomain/GroupDocs.Viewer.UI.Sample.AzureCustomDomain.csproj"

COPY . .
WORKDIR "/src/samples/GroupDocs.Viewer.UI.Sample.AzureCustomDomain"

RUN dotnet build "./GroupDocs.Viewer.UI.Sample.AzureCustomDomain.csproj" -c $BUILD_CONFIGURATION -o /app/build


# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupDocs.Viewer.UI.Sample.AzureCustomDomain.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GroupDocs.Viewer.UI.Sample.AzureCustomDomain.dll"]
