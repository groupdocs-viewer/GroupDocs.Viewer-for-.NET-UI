# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GroupDocs.Viewer.UI is a modular .NET library that provides a document viewer UI (embedded Angular SPA) with pluggable API backends, storage providers, and cache providers. Targets .NET 6.0 and .NET 8.0.

## Build Commands

```bash
dotnet build ./GroupDocs.Viewer.UI.sln                    # Build entire solution
dotnet test ./tests/GroupDocs.Viewer.UI.Api.Tests          # Run all tests
dotnet test ./tests/GroupDocs.Viewer.UI.Api.Tests --filter "FullyQualifiedName~ClassName.MethodName"  # Run single test
```

Production packaging is done via `build.ps1` (PowerShell), which packs all 12 NuGet packages into `./build_out/`. CI runs on Windows with .NET 6.0 and 8.0.

## Architecture

The solution is layered into three tiers connected via dependency injection:

**Core layer** (`GroupDocs.Viewer.UI.Core`, netstandard2.0): Defines interfaces (`IFileStorage`, `IFileCache`) and configuration types (`Config`, `RenderingMode`, `ZoomLevel`). No ASP.NET dependency.

**API layer** (`GroupDocs.Viewer.UI.Api`, net6.0/net8.0): Convention-based controllers, URL building, error handling. Endpoints: `/get-page`, `/get-thumb`, `/get-pdf`, `/get-resource`.

**UI layer** (`GroupDocs.Viewer.UI`): Serves the Angular SPA from embedded resources at a configurable path (default `/viewer`).

### API Implementations (pick one)

- `SelfHost.Api` — Windows rendering via GroupDocs.Viewer + System.Drawing.Common
- `SelfHost.Api.CrossPlatform` — Linux-compatible rendering without System.Drawing
- `Cloud.Api` — Delegates to GroupDocs Cloud API

`SelfHost.Api` and `SelfHost.Api.CrossPlatform` share code via `SelfHost.Api.Shared` (MSBuild shared project).

### Storage Providers (pick one)

`Api.Local.Storage` | `Api.Cloud.Storage` | `Api.AzureBlob.Storage` | `Api.AwsS3.Storage`

### Cache Providers (pick one)

`Api.Local.Cache` | `Api.InMemory.Cache`

### Rendering Modes

- `RenderingMode.Html` (default) — documents as HTML with resources
- `RenderingMode.Image` — documents as PNG/JPG images

## Key Patterns

- **Extension methods** for DI registration live in namespace `Microsoft.Extensions.DependencyInjection` and `Microsoft.AspNetCore.Builder`
- **Options pattern** via `IOptions<T>` for configuration
- **Fluent builder** for setup: `AddGroupDocsViewerUI(config => { ... })`
- All packages share synchronized version numbers (currently 8.1.5), defined in `build/dependencies.props`
- Package versions for dependencies are centralized in `build/dependencies.props`
- `global.json` pins SDK to 6.0.400 with `rollForward: latestMajor`

## Tests

xUnit with Moq. Tests are in `tests/GroupDocs.Viewer.UI.Api.Tests/` and cover URL building logic (`ApiUrlBuilder`). CI collects XPlat Code Coverage via Coverlet.
