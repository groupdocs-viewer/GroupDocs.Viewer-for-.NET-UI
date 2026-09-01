# GroupDocs.Viewer.UI.SelfHost.Api.Shared

`GroupDocs.Viewer.UI.SelfHost.Api.Shared` is a shared library that provides core functionality for document viewing the `GroupDocs.Viewer.UI.SelfHost.Api` implementation. This library contains the common components that enable document viewing capabilities across platforms.

## Purpose

This shared library serves as the foundation for `GroupDocs.Viewer.UI.SelfHost.Api`. It provides:

- Common configuration management
- Base viewer implementations
- Internal caching mechanisms
- License management
- File type resolution
- Resource handling

## API Components

### Configuration Management
The library provides a unified configuration system that allows setting up:
- Viewer type selection (HTML, PNG, JPG)
- License management
- View options for different output formats
- Internal caching behavior
- Spreadsheet-specific rendering options

### Viewer System
The shared viewer system includes:
- Base viewer implementation with common functionality
- Support for multiple viewer types
- Document information retrieval
- Page rendering capabilities
- Thumbnail generation
- PDF export functionality

### Internal Caching
The caching system provides:
- Memory-based storage
- Configurable expiration policies
- Thread-safe operations
- Automatic resource cleanup
- Performance optimization for frequently accessed documents

### License Management
The license system supports:
- Multiple license sources (file, environment variables)
- Error handling and logging
- Evaluation mode fallback

## Usage

This library is not meant to be used directly in applications. Instead, it serves as the foundation for:

1. `GroupDocs.Viewer.UI.SelfHost.Api` - the self-hosted API implementation, cross-platform as of 26.8.0

That implementation builds upon this shared library to provide its functionality while maintaining consistent behavior for core document viewing operations.

## Technical Details

The shared library ensures that both implementations:
- Handle documents consistently
- Provide the same configuration options
- Manage resources efficiently
- Support the same viewer types
- Implement identical caching strategies
- Handle licensing in the same way

This shared foundation allows developers to switch between implementations while maintaining the same API surface and behavior.

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 