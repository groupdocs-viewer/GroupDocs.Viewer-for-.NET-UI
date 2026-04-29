# Custom File Storage Sample

This sample demonstrates how to implement and register a custom `IFileStorage` provider, replacing the built-in local, cloud, Azure Blob, or AWS S3 storage with your own implementation.

The sample is shaped around a common scenario: **files are stored as BLOBs in a database** and need to be served to the viewer. To keep the sample free of extra dependencies, the database is simulated with an in-memory `Dictionary<string, byte[]>` seeded from `./Files/` at startup. In a real application the seeding would be replaced by a SQL query against your BLOB column.

## What This Sample Shows

- Implementing the `IFileStorage` interface with a custom `DatabaseFileStorage`
- Implementing **`ReadFileStreamAsync`** — required since GroupDocs.Viewer.UI **26.3.0**, which switched the SelfHost viewer to call this method instead of `ReadFileAsync`
- Registering the custom storage provider after `AddGroupDocsViewerSelfHostApi()` to override the default

## Why `ReadFileStreamAsync`?

In version 26.3.0 the `IFileStorage` interface gained a new method:

```csharp
Task<Stream> ReadFileStreamAsync(string filePath, CancellationToken cancellationToken = default);
```

…and the SelfHost viewer was changed to call it directly in `BaseViewer.cs`:

```csharp
private async Task<Stream> GetFileStreamAsync(string filePath)
{
    return await _fileStorage.ReadFileStreamAsync(filePath);
}
```

Previously, the viewer called `ReadFileAsync` and wrapped the resulting `byte[]` in a new `MemoryStream`. The change avoids buffering the whole file in memory — important for large documents.

There is **no flag** in `Config`, in any options class, or in the DI extension methods that toggles this back. Custom `IFileStorage` implementations must implement `ReadFileStreamAsync` to keep working under 26.3.0+.

## Prerequisites

- .NET 8.0 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.CustomFileStorage
dotnet run
```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

The custom storage is registered as a singleton after the self-host API setup:

```csharp
builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config => { ... })
    .AddLocalCache("./Cache");

// Register custom storage AFTER AddGroupDocsViewerSelfHostApi()
builder.Services.AddSingleton<IFileStorage>(_ => new DatabaseFileStorage("./Files"));
```

**Important:** The custom `IFileStorage` registration must come after `AddGroupDocsViewerSelfHostApi()` to override any default storage implementation.

## Implementing IFileStorage

The `IFileStorage` interface requires four methods:

```csharp
public interface IFileStorage
{
    Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath, CancellationToken ct = default);
    Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default);
    Task<Stream> ReadFileStreamAsync(string filePath, CancellationToken ct = default);
    Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite, CancellationToken ct = default);
}
```

### Two viable patterns for `ReadFileStreamAsync`

| Pattern | When to use | Trade-off |
| --- | --- | --- |
| **`MemoryStream` over `byte[]`** (this sample) | Your data access layer hands you a `byte[]` — EF Core, Dapper with a `byte[]` column, in-memory store, REST API returning bytes. | The whole file is buffered in memory. Same cost as the pre-26.3.0 behaviour. |
| **True streaming** | Large BLOBs in SQL Server / Postgres, where you want to avoid the buffer. | Use ADO.NET's `SqlDataReader.GetStream(ordinal)` with `CommandBehavior.SequentialAccess` and keep the connection / reader alive until the consumer disposes the stream. |

The bundled `CloudFileStorage.ReadFileStreamAsync` uses the `MemoryStream` pattern, so it is officially supported.

This pattern can also be adapted to any other backend — REST APIs, custom file systems, encrypted containers, etc. — by replacing the dictionary lookup with the appropriate I/O call.
