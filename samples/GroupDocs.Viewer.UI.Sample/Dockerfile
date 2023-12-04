#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt update && apt install -y \
    libgdiplus \
    libc6-dev \
    libx11-dev \
    fontconfig \
    ttf-mscorefonts-installer

RUN fc-cache -f -v

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Nuget.config", "."]
COPY ["samples/GroupDocs.Viewer.UI.Sample/GroupDocs.Viewer.UI.Sample.csproj", "samples/GroupDocs.Viewer.UI.Sample/"]
COPY ["src/GroupDocs.Viewer.UI/GroupDocs.Viewer.UI.csproj", "src/GroupDocs.Viewer.UI/"]
COPY ["src/GroupDocs.Viewer.UI.Api/GroupDocs.Viewer.UI.Api.csproj", "src/GroupDocs.Viewer.UI.Api/"]
COPY ["src/GroupDocs.Viewer.UI.Core/GroupDocs.Viewer.UI.Core.csproj", "src/GroupDocs.Viewer.UI.Core/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Cache/GroupDocs.Viewer.UI.Api.Local.Cache.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Cache/"]
COPY ["src/GroupDocs.Viewer.UI.Api.Local.Storage/GroupDocs.Viewer.UI.Api.Local.Storage.csproj", "src/GroupDocs.Viewer.UI.Api.Local.Storage/"]
COPY ["src/GroupDocs.Viewer.UI.SelfHost.Api/GroupDocs.Viewer.UI.SelfHost.Api.csproj", "src/GroupDocs.Viewer.UI.SelfHost.Api/"]
RUN dotnet restore "samples/GroupDocs.Viewer.UI.Sample/GroupDocs.Viewer.UI.Sample.csproj"
COPY . .
WORKDIR "/src/samples/GroupDocs.Viewer.UI.Sample"
RUN dotnet build "GroupDocs.Viewer.UI.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GroupDocs.Viewer.UI.Sample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroupDocs.Viewer.UI.Sample.dll"]