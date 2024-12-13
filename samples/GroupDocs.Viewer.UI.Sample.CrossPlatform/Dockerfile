# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project and dependency files
COPY ["Directory.Build.props", "."]
COPY ["Nuget.config", "."]
COPY ["samples/GroupDocs.Viewer.UI.Sample.CrossPlatform/GroupDocs.Viewer.UI.Sample.CrossPlatform.csproj", "samples/GroupDocs.Viewer.UI.Sample.CrossPlatform/"]
COPY ["src/GroupDocs.Viewer.UI/GroupDocs.Viewer.UI.csproj", "src/GroupDocs.Viewer.UI/"]
COPY ["src/GroupDocs.Viewer.UI.Api/GroupDocs.Viewer.UI.Api.csproj", "src/GroupDocs.Viewer.UI.Api/"]
COPY ["src/GroupDocs.Viewer.UI.Core/GroupDocs.Viewer.UI.Core.csproj", "src/GroupDocs.Viewer.UI.Core/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Cache/GroupDocs.Viewer.UI.Api.Local.Cache.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Cache/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Storage/GroupDocs.Viewer.UI.Api.Local.Storage.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Storage/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform.csproj", "src/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform/"]

# Restore dependencies
RUN dotnet restore "./samples/GroupDocs.Viewer.UI.Sample.CrossPlatform/GroupDocs.Viewer.UI.Sample.CrossPlatform.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/samples/GroupDocs.Viewer.UI.Sample.CrossPlatform"

# Build the project
RUN dotnet build "./GroupDocs.Viewer.UI.Sample.CrossPlatform.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupDocs.Viewer.UI.Sample.CrossPlatform.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entry point for the application
ENTRYPOINT ["dotnet", "GroupDocs.Viewer.UI.Sample.CrossPlatform.dll"]
