# GroupDocs.Viewer.UI.Api.AwsS3.Storage

`GroupDocs.Viewer.UI.Api.AwsS3.Storage` is an Amazon S3 file storage implementation that can be used with `GroupDocs.Viewer.UI.Api`. It provides seamless integration with AWS S3 for storing and retrieving documents in your `GroupDocs.Viewer.UI` application.

## Installation

To use AWS S3 storage in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Api.AwsS3.Storage
```

2. Configure AWS S3 storage in your `Startup` class:

```cs
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddAwsS3Storage(options =>
    {
        options.Region = "us-east-1"; // AWS region
        options.BucketName = "your-bucket-name";
        options.AccessKey = "your-access-key"; // Optional if using AWS credentials
        options.SecretKey = "your-secret-key"; // Optional if using AWS credentials
        options.S3Config = new AmazonS3Config
        {
            // Configure additional S3 client settings if needed
        };
    });

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.ApiEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
        });
    });

await app.RunAsync();
```

## Configuration Options

The AWS S3 storage implementation supports the following configuration options:

- `Region` (Required): The AWS region where your S3 bucket is located (e.g., "us-east-1", "eu-west-1").
- `BucketName` (Required): The name of your S3 bucket.
- `AccessKey` (Optional): AWS access key. If not provided, the AWS SDK will use the default credential provider chain.
- `SecretKey` (Optional): AWS secret key. If not provided, the AWS SDK will use the default credential provider chain.
- `S3Config` (Optional): Additional configuration for the Amazon S3 client.

## AWS Credentials

You can provide AWS credentials in several ways:

1. **Explicit Configuration**:
   ```cs
   options.AccessKey = "your-access-key";
   options.SecretKey = "your-secret-key";
   ```

2. **Environment Variables**:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`

3. **AWS Credentials File**:
   - Located at `~/.aws/credentials` (Linux/Mac) or `%UserProfile%\.aws\credentials` (Windows)

4. **Instance Profile** (when running on AWS EC2)

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 