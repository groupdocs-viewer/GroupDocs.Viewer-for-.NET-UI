# Cloud API Sample

This sample demonstrates using the GroupDocs Cloud API for document rendering instead of the local self-host API. Documents are rendered server-side via the GroupDocs Cloud service, so no local rendering engine is required.

## What This Sample Shows

- Configuring `AddGroupDocsViewerCloudApi()` with Cloud API credentials
- Using the Cloud API backend for document rendering
- Local file storage and cache with cloud-based rendering

## Prerequisites

- .NET 8.0 SDK or later
- A GroupDocs Cloud account with API credentials (Client ID and Client Secret)

## Getting Cloud API Credentials

1. Sign up at [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud)
2. Create an application to get your **Client ID** and **Client Secret**

## Running

1. **Update credentials** in `Program.cs`:
   ```csharp
   var clientId = "your-client-id";
   var clientSecret = "your-client-secret";
   ```

2. **Run the sample:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.Cloud.Api.Sample
   dotnet run
   ```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

```csharp
builder.Services
    .AddControllers()
    .AddGroupDocsViewerCloudApi(config =>
        config
            .SetClientId(clientId)
            .SetClientSecret(clientSecret)
            .SetViewerType(viewerType)
    )
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");
```

## Adding Documents

Place document files in the `./Files` directory. They will be uploaded to the Cloud API for rendering.
