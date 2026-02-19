# Cross-Platform Sample

This sample demonstrates running GroupDocs.Viewer.UI on Linux and macOS using the cross-platform rendering API, which does not depend on `System.Drawing.Common` or GDI+.

## What This Sample Shows

- Using `AddGroupDocsViewerSelfHostApi()` with the cross-platform backend (via the `CrossPlatform` project reference)
- Self-host rendering that works on Linux, macOS, and Windows without GDI+ dependencies
- Standard local storage and cache setup

## Prerequisites

- .NET 8.0 SDK or later
- Works on Windows, Linux, and macOS

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.CrossPlatform
dotnet run
```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

The setup is identical to the basic sample, but the `.csproj` references `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` instead of `GroupDocs.Viewer.UI.SelfHost.Api`:

```xml
<ProjectReference Include="..\..\src\GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform\..." />
```

This swaps in the cross-platform rendering engine that avoids `System.Drawing.Common`.

## When to Use This

- Deploying to Linux containers or App Services
- Running on macOS for development
- Any environment where GDI+ / `libgdiplus` is not available or not desired

For Windows-only deployments, the standard `SelfHost.Api` (used in the basic sample) provides the same functionality.
