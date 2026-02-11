# Azure App Service Custom Domain Sample

This sample demonstrates how to deploy and run GroupDocs.Viewer.UI on Azure App Service with custom domains. The sample is configured to generate relative URLs, ensuring that custom domains work correctly without requiring additional configuration.

## Running Locally

### Docker Compose with Nginx (Simulates Azure Custom Domain)

This setup simulates Azure App Service with a custom domain using Nginx as a reverse proxy:

1. **Navigate to the sample directory:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.Sample.AzureCustomDomain
   ```

2. **Start with Docker Compose:**
   ```bash
   docker-compose up --build
   ```

3. **Configure custom domain locally:**
   - Add to your `hosts` file:
     - **Windows**: `C:\Windows\System32\drivers\etc\hosts`
     - **Linux/Mac**: `/etc/hosts`
   - Add this line:
     ```
     127.0.0.1 custom-domain.local
     ```

4. **Access via custom domain:**
   - Open browser: `http://custom-domain.local:8080/viewer`
   - Or access directly: `http://localhost:5000/viewer` (bypasses nginx)

This setup allows you to test custom domain behavior locally before deploying to Azure.

## Overview

When deploying to Azure App Service with a custom domain, GroupDocs.Viewer.UI generates relative URLs by default, which automatically respect the browser's current domain. This means your application will work seamlessly whether accessed via:
- Azure's default hostname: `https://app-name.azurewebsites.net`
- Your custom domain: `https://your-custom-domain.com`

## Configuration

The sample is pre-configured to use relative URLs (default behavior):

```csharp
endpoints.MapGroupDocsViewerApi(options =>
{
    options.ApiPath = "/viewer-api";
    // UseAbsoluteUrls defaults to false, generating relative URLs
    // This ensures URLs respect the browser's domain automatically
});
```

### Using Absolute URLs (Optional)

If you need absolute URLs for cross-domain scenarios or CDN integration, you can enable them:

```csharp
endpoints.MapGroupDocsViewerApi(options =>
{
    options.UseAbsoluteUrls = true;
    options.ApiDomain = "https://your-custom-domain.com"; // Your Azure custom domain
    options.ApiPath = "/viewer-api";
});
```

## Deploying to Azure App Service

### Prerequisites

- Azure account with App Service access
- Azure CLI or Visual Studio with Azure tools installed
- .NET 8.0 SDK

### Deployment Steps

1. **Create an Azure App Service:**
   ```bash
   az webapp create --resource-group <your-resource-group> \
                    --plan <your-app-service-plan> \
                    --name <your-app-name> \
                    --runtime "DOTNET|8.0"
   ```

2. **Configure custom domain** (if needed):
   - In Azure Portal, go to your App Service
   - Navigate to **Custom domains**
   - Add your custom domain and configure DNS records

3. **Deploy the application:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.Sample.AzureCustomDomain
   dotnet publish -c Release
   az webapp deploy --resource-group <your-resource-group> \
                    --name <your-app-name> \
                    --src-path bin/Release/net8.0/publish
   ```

   Or use Visual Studio's **Publish** feature to deploy directly.

4. **Access your application:**
   - Via Azure hostname: `https://<your-app-name>.azurewebsites.net/viewer`
   - Via custom domain: `https://<your-custom-domain>/viewer`

### Important Notes for Azure Deployment

- **Relative URLs**: The sample uses relative URLs by default, so it works automatically with custom domains
- **HTTPS**: Azure App Service provides HTTPS by default. Ensure your custom domain SSL certificate is configured
- **Environment Variables**: You can set `ASPNETCORE_ENVIRONMENT` and other variables in Azure Portal under **Configuration**
- **Files and Cache**: Consider using Azure Blob Storage for files and Azure Cache for Redis for caching in production

## Verifying Relative URLs

After deployment, verify that relative URLs are being generated:

1. Open browser developer tools (F12)
2. Navigate to the Network tab
3. Access a document via the viewer UI
4. Verify all API requests use relative URLs like `/viewer-api/get-page?file=document.docx&page=1`
5. They should NOT contain absolute URLs with hostnames

This confirms that the application will work correctly with your custom domain.

## Related Documentation

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure Custom Domains](https://docs.microsoft.com/azure/app-service/app-service-web-tutorial-custom-domain)
