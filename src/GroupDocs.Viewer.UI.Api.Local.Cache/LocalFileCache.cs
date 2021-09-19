using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using GroupDocs.Viewer.UI.Core;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Local.Cache
{
    public class LocalFileCache : IFileCache
    {
        /// <summary>
        /// The Relative or absolute path to the cache folder.
        /// </summary>
        private string CachePath { get; }

        private readonly TimeSpan _waitTimeout = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Creates new instance of <see cref="LocalFileCache"/> class.
        /// </summary>
        /// <param name="cachePath">Relative or absolute path where document cache will be stored.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="cachePath"/> is null.</exception>
        public LocalFileCache(string cachePath)
        {
            if (cachePath == null)
                throw new ArgumentNullException(nameof(cachePath));

            CachePath = cachePath;
        }

        /// <summary>
        /// Deserializes data associated with this key if present.
        /// </summary>
        /// <param name="cacheKey">A key identifying the requested entry.</param>
        /// <param name="filePath">The relative or absolute filepath.</param>
        /// <returns><code>True</code> if the key was found.</returns>
        public TEntry TryGetValue<TEntry>(string cacheKey, string filePath)
        {
            string cacheFilePath = GetCacheFilePath(cacheKey, filePath);

            if (File.Exists(cacheFilePath))
            {
                if (typeof(TEntry) == typeof(byte[]))
                    return (TEntry)ReadBytes(cacheFilePath);

                if (typeof(TEntry) == typeof(Stream))
                    return (TEntry)ReadStream(cacheFilePath);

                return Deserialize<TEntry>(cacheFilePath);
            }

            return default(TEntry);
        }

        /// <summary>
        /// Deserializes data associated with this key if present.
        /// </summary>
        /// <param name="cacheKey">A key identifying the requested entry.</param>
        /// <param name="filePath">The relative or absolute filepath.</param>
        /// <returns><code>True</code> if the key was found.</returns>
        public async Task<TEntry> TryGetValueAsync<TEntry>(string cacheKey, string filePath)
        {
            string cacheFilePath = GetCacheFilePath(cacheKey, filePath);

            if (File.Exists(cacheFilePath))
            {
                if (typeof(TEntry) == typeof(byte[]))
                    return (TEntry)ReadBytes(cacheFilePath);

                if (typeof(TEntry) == typeof(Stream))
                    return (TEntry)ReadStream(cacheFilePath);

                return await DeserializeAsync<TEntry>(cacheFilePath);
            }

            return default(TEntry);
        }

        /// <summary>
        /// Serializes data to the local disk.
        /// </summary>
        /// <param name="cacheKey">An unique identifier for the cache entry.</param>
        /// <param name="filePath">The relative or absolute filepath.</param>
        /// <param name="value">The object to serialize.</param>
        public void Set<TEntry>(string cacheKey, string filePath, TEntry value)
        {
            if (value == null)
                return;

            string cacheFilePath = GetCacheFilePath(cacheKey, filePath);

            if (value is byte[] data)
            {
                using FileStream dst = GetStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                dst.Write(data, 0, data.Length);
            }
            else if (value is Stream src)
            {
                using FileStream dst = GetStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                if (src.CanSeek)
                    src.Position = 0;

                src.CopyTo(dst);
            }
            else
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, options);

                using FileStream stream = GetStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// Serializes data to the local disk.
        /// </summary>
        /// <param name="cacheKey">An unique identifier for the cache entry.</param>
        /// <param name="filePath">The relative or absolute filepath.</param>
        /// <param name="value">The object to serialize.</param>
        public async Task SetAsync<TEntry>(string cacheKey, string filePath, TEntry value)
        {
            if (value == null)
                return;

            string cacheFilePath = GetCacheFilePath(cacheKey, filePath);

            if (value is byte[] data)
            {
                await using FileStream dst = GetStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await dst.WriteAsync(data, 0, data.Length);
            }
            else if (value is Stream src)
            {
                await using FileStream dst = GetStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                if (src.CanSeek)
                    src.Position = 0;

                await src.CopyToAsync(dst);
            }
            else
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var bytes = JsonSerializer.SerializeToUtf8Bytes(value, options);

                await using FileStream stream = GetStream(cacheFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        private object ReadStream(string cacheFilePath)
            => GetStream(cacheFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        private object ReadBytes(string cacheFilePath)
            => GetBytes(cacheFilePath);

        private TEntry Deserialize<TEntry>(string cachePath)
        {
            object data;
            try
            {
                var bytes = GetBytes(cachePath);
                data = JsonSerializer.Deserialize<TEntry>(bytes);
            }
            catch (SerializationException)
            {
                data = default(TEntry);
            }

            return (TEntry)data;
        }

        private async Task<TEntry> DeserializeAsync<TEntry>(string cachePath)
        {
            object data;
            try
            {
                await using var stream = GetStream(cachePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                data = await JsonSerializer.DeserializeAsync<TEntry>(stream);
            }
            catch (SerializationException)
            {
                data = default(TEntry);
            }

            return (TEntry)data;
        }

        private string GetCacheFilePath(string cacheKey, string filePath)
        {
            string cacheSubFolder = string.Join("_", filePath.Split(Path.GetInvalidPathChars()))
                .Replace(".", "_");
            string cacheDirPath = Path.Combine(CachePath, cacheSubFolder);
            string cacheFilePath = Path.Combine(cacheDirPath, cacheKey);

            if (!Directory.Exists(cacheDirPath))
                Directory.CreateDirectory(cacheDirPath);

            return cacheFilePath;
        }

        private FileStream GetStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            FileStream stream = null;
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, 50);
            TimeSpan totalTime = new TimeSpan();

            while (stream == null)
            {
                try
                {
                    stream = File.Open(path, mode, access, share);
                }
                catch (IOException)
                {
                    Thread.Sleep(interval);
                    totalTime += interval;

                    if (_waitTimeout.Ticks != 0 && totalTime > _waitTimeout)
                    {
                        throw;
                    }
                }
            }

            return stream;
        }

        private byte[] GetBytes(string path)
        {
            byte[] bytes = null;
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, 50);
            TimeSpan totalTime = new TimeSpan();

            while (bytes == null)
            {
                try
                {
                    bytes = File.ReadAllBytes(path);
                }
                catch (IOException)
                {
                    Thread.Sleep(interval);
                    totalTime += interval;

                    if (_waitTimeout.Ticks != 0 && totalTime > _waitTimeout)
                    {
                        throw;
                    }
                }
            }

            return bytes;
        }
    }
}