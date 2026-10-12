using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Local.Storage;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Storage
{
    /// <summary>
    /// Tests that paths received from the client can't point outside the storage folder.
    /// Layout: {base}/Files is the storage folder, {base}/Secret and {base}/Files2 are outside of it.
    /// </summary>
    public class LocalFileStorageTests : IDisposable
    {
        private readonly string _basePath;
        private readonly string _storagePath;
        private readonly LocalFileStorage _storage;

        public LocalFileStorageTests()
        {
            _basePath = Path.Combine(Path.GetTempPath(), $"viewer_storage_test_{Guid.NewGuid():N}");
            _storagePath = Path.Combine(_basePath, "Files");

            CreateFile(Path.Combine(_storagePath, "a.txt"), "a");
            CreateFile(Path.Combine(_storagePath, "sub", "inner.txt"), "inner");
            CreateFile(Path.Combine(_basePath, "Secret", "secret.txt"), "secret");
            CreateFile(Path.Combine(_basePath, "Files2", "other.txt"), "other");

            _storage = new LocalFileStorage(_storagePath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_basePath))
                Directory.Delete(_basePath, recursive: true);
        }

        [Theory]
        [InlineData("")]
        [InlineData(".")]
        [InlineData("/")]
        public async Task ListDirsAndFilesAsync_WithStorageFolder_ShouldListStorageFolder(string dirPath)
        {
            var entries = await _storage.ListDirsAndFilesAsync(dirPath);

            Assert.Equal(new[] { "sub", "a.txt" }, entries.Select(entry => entry.FilePath));
        }

        [Fact]
        public async Task ListDirsAndFilesAsync_WithNestedFolder_ShouldReturnRelativePaths()
        {
            var entries = await _storage.ListDirsAndFilesAsync("sub");

            var entry = Assert.Single(entries);
            Assert.Equal(Path.Combine("sub", "inner.txt"), entry.FilePath);
        }

        [Theory]
        [InlineData("..")]
        [InlineData("../Secret")]
        [InlineData("sub/../..")]
        public async Task ListDirsAndFilesAsync_WithPathOutsideStorage_ShouldThrow(string dirPath)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ListDirsAndFilesAsync(dirPath));
        }

        [Theory]
        [InlineData("a.txt", "a")]
        [InlineData("/a.txt", "a")]
        [InlineData("sub/inner.txt", "inner")]
        [InlineData("/sub/inner.txt", "inner")]
        [InlineData("sub/../a.txt", "a")]
        public async Task ReadFileAsync_WithPathInsideStorage_ShouldReturnContent(string filePath, string expected)
        {
            var bytes = await _storage.ReadFileAsync(filePath);

            Assert.Equal(expected, Encoding.UTF8.GetString(bytes));
        }

        [Theory]
        [InlineData("../Secret/secret.txt")]
        [InlineData("sub/../../Secret/secret.txt")]
        [InlineData("../Files2/other.txt")]
        public async Task ReadFileAsync_WithPathOutsideStorage_ShouldThrow(string filePath)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileAsync(filePath));
        }

        [Fact]
        public async Task ReadFileAsync_WithNullCharInPath_ShouldThrow()
        {
            var filePath = "a" + '\0' + ".txt";

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileAsync(filePath));
        }

        [Fact]
        public async Task ReadFileAsync_WithAbsolutePath_ShouldNotReadOutsideStorage()
        {
            var secretPath = Path.Combine(_basePath, "Secret", "secret.txt");

            if (OperatingSystem.IsWindows())
            {
                await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileAsync(secretPath));
            }
            else
            {
                // A leading "/" refers to the storage folder, so the file is looked up inside it
                await Assert.ThrowsAnyAsync<IOException>(() => _storage.ReadFileAsync(secretPath));
            }
        }

        [Fact]
        public async Task ReadFileAsync_WithRootRelativePath_ShouldLookUpFileInsideStorage()
        {
            var secretPath = Path.Combine(_basePath, "Secret", "secret.txt");
            var rootRelativePath = Path.DirectorySeparatorChar + secretPath.Substring(Path.GetPathRoot(secretPath)!.Length);

            await Assert.ThrowsAnyAsync<IOException>(() => _storage.ReadFileAsync(rootRelativePath));
        }

        [Theory]
        [InlineData("..\\Secret\\secret.txt")]
        [InlineData("sub\\..\\..\\Secret\\secret.txt")]
        [InlineData("C:secret.txt")]
        public async Task ReadFileAsync_WithWindowsPathOutsideStorage_ShouldThrow(string filePath)
        {
            // Backslashes and drive letters are plain file name characters on Linux and macOS
            if (!OperatingSystem.IsWindows())
                return;

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileAsync(filePath));
        }

        [Fact]
        public async Task ReadFileAsync_WithUncPath_ShouldLookUpFileInsideStorage()
        {
            if (!OperatingSystem.IsWindows())
                return;

            CreateFile(Path.Combine(_storagePath, "server", "share", "doc.txt"), "inside");

            var bytes = await _storage.ReadFileAsync(@"\\server\share\doc.txt");

            Assert.Equal("inside", Encoding.UTF8.GetString(bytes));
        }

        [Fact]
        public async Task ReadFileAsync_WhenPathIsOutsideStorage_ShouldNotExposePathInMessage()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _storage.ReadFileAsync("../Secret/password-secret.txt"));

            Assert.DoesNotContain("Secret", exception.Message);
            // The controller returns 403 "Password Required" for messages that mention a password
            Assert.DoesNotContain("password", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ReadFileStreamAsync_WithNestedPath_ShouldReturnContent()
        {
            await using var stream = await _storage.ReadFileStreamAsync("sub/inner.txt");
            using var reader = new StreamReader(stream);

            Assert.Equal("inner", await reader.ReadToEndAsync());
        }

        [Theory]
        [InlineData("../Secret/secret.txt")]
        [InlineData("../Files2/other.txt")]
        public async Task ReadFileStreamAsync_WithPathOutsideStorage_ShouldThrow(string filePath)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileStreamAsync(filePath));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteFileAsync_WithPathOutsideStorage_ShouldThrowAndNotCreateFile(bool rewrite)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _storage.WriteFileAsync("../evil.txt", new byte[] { 1 }, rewrite));

            Assert.False(File.Exists(Path.Combine(_basePath, "evil.txt")));
        }

        [Fact]
        public async Task WriteFileAsync_WithAbsolutePath_ShouldNotCreateFileOutsideStorage()
        {
            var targetPath = Path.Combine(_basePath, "Secret", "evil.txt");

            if (OperatingSystem.IsWindows())
            {
                await Assert.ThrowsAsync<UnauthorizedAccessException>(
                    () => _storage.WriteFileAsync(targetPath, new byte[] { 1 }, rewrite: true));
            }
            else
            {
                // A leading "/" refers to the storage folder, which has no such sub-folders
                await Assert.ThrowsAnyAsync<IOException>(
                    () => _storage.WriteFileAsync(targetPath, new byte[] { 1 }, rewrite: true));
            }

            Assert.False(File.Exists(targetPath));
        }

        [Fact]
        public async Task WriteFileAsync_WhenFileExists_ShouldReturnFreeFileName()
        {
            var fileName = await _storage.WriteFileAsync("a.txt", new byte[] { 1 }, rewrite: false);

            Assert.Equal("a (1).txt", fileName);
            Assert.True(File.Exists(Path.Combine(_storagePath, "a (1).txt")));
        }

        [Fact]
        public async Task WriteFileAsync_WithNestedPath_ShouldWriteInsideStorage()
        {
            var fileName = await _storage.WriteFileAsync("sub/new.txt", new byte[] { 1 }, rewrite: true);

            Assert.Equal("sub/new.txt", fileName);
            Assert.True(File.Exists(Path.Combine(_storagePath, "sub", "new.txt")));
        }

        private static void CreateFile(string filePath, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, content);
        }
    }
}
