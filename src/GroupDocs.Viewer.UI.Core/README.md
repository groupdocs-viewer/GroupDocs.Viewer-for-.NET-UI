# GroupDocs.Viewer.UI.Core

`GroupDocs.Viewer.UI.Core` is a foundational library that provides core interfaces, types, and utilities for the GroupDocs.Viewer.UI ecosystem. This package is typically not referenced directly by end users but serves as the base for other GroupDocs.Viewer.UI packages.

## Overview

This package contains:

1. Core interfaces and abstract classes
2. Common data models and entities
3. Configuration types
4. Extension methods
5. Builder classes

## Key Components

### Core Interfaces
- `IViewer`: Defines the contract for document viewing operations
- `IFileStorage`: Specifies file storage operations
- `IFileCache`: Defines caching operations
- `IPageFormatter`: Provides page formatting capabilities
- `IAsyncLock`: Manages asynchronous locking

### Data Models
- `DocumentInfo`: Represents document metadata
- `Page`: Base class for document pages
- `Thumb`: Base class for page thumbnails
- `FileCredentials`: Contains file access information
- `FileSystemEntry`: Represents file system items

### Configuration
- `Config`: Contains UI configuration options
- `ViewerType`: Defines supported viewer types
- `RenderingMode`: Specifies rendering modes

## Usage

This package is primarily used by other GroupDocs.Viewer UI packages:

- `GroupDocs.Viewer.UI`
- `GroupDocs.Viewer.UI.Api`
- `GroupDocs.Viewer.UI.Cloud.Api`
- `GroupDocs.Viewer.UI.SelfHost.Api`

Developers typically don't need to reference this package directly. Instead, they should use one of the higher-level packages that implement these interfaces.

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 