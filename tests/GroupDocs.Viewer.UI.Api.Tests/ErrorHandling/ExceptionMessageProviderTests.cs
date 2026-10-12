using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.ErrorHandling
{
    public class ExceptionMessageProviderTests : IDisposable
    {
        private readonly string _tempPath;
        private readonly ExceptionMessageProvider _provider = new ExceptionMessageProvider();
        private readonly ErrorContext _context = new ErrorContext("view-data", new DefaultHttpContext());

        public ExceptionMessageProviderTests()
        {
            _tempPath = Path.Combine(Path.GetTempPath(), $"viewer_errors_test_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempPath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempPath))
                Directory.Delete(_tempPath, recursive: true);
        }

        [Fact]
        public void GetErrorMessage_WhenFileIsNotFound_ShouldReturnFileNameOnly()
        {
            var exception = Record.Exception(() => File.ReadAllBytes(Path.Combine(_tempPath, "missing.docx")));

            var message = _provider.GetErrorMessage(exception, _context);

            Assert.IsType<FileNotFoundException>(exception);
            Assert.Equal("File 'missing.docx' was not found.", message);
        }

        [Fact]
        public void GetErrorMessage_WhenFileIsNotFoundWithoutFileName_ShouldReturnGenericMessage()
        {
            var exception = new FileNotFoundException("'missing.docx' not found in the database.");

            var message = _provider.GetErrorMessage(exception, _context);

            Assert.Equal("File was not found.", message);
        }

        [Fact]
        public void GetErrorMessage_WhenFolderIsNotFound_ShouldNotExposePath()
        {
            var exception = Record.Exception(() => File.ReadAllBytes(Path.Combine(_tempPath, "missing", "file.docx")));

            var message = _provider.GetErrorMessage(exception, _context);

            Assert.IsType<DirectoryNotFoundException>(exception);
            Assert.Equal("File or folder was not found.", message);
        }

        [Fact]
        public void GetErrorMessage_WhenAccessIsDenied_ShouldNotExposePath()
        {
            var exception = new UnauthorizedAccessException($"Access to the path '{_tempPath}' is denied.");

            var message = _provider.GetErrorMessage(exception, _context);

            Assert.Equal("Access to the file or folder is denied.", message);
        }

        [Fact]
        public void GetErrorMessage_WhenFileIsLocked_ShouldNotExposePath()
        {
            var filePath = Path.Combine(_tempPath, "locked.docx");
            var exception = new IOException($"The process cannot access the file '{filePath}' because it is being used by another process.");

            var message = _provider.GetErrorMessage(exception, _context);

            Assert.Equal("File could not be accessed.", message);
        }

        [Fact]
        public void GetErrorMessage_WithOtherException_ShouldReturnExceptionMessage()
        {
            var exception = new InvalidOperationException("The document is corrupted.");

            var message = _provider.GetErrorMessage(exception, _context);

            Assert.Equal("The document is corrupted.", message);
        }
    }
}
