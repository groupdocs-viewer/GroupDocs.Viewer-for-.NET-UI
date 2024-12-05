using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GroupDocs.Viewer.UI.Core
{
    internal class UIEmbeddedResourcesReader
        : IUIResourcesReader
    {
        private readonly Assembly _assembly;
        private readonly string _embeddedResourcePrefix;

        private IReadOnlyCollection<UIResource> _cachedUiResources = null;

        public UIEmbeddedResourcesReader(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _embeddedResourcePrefix = $"{_assembly.GetName().Name}.App.";
        }

        public IReadOnlyCollection<UIResource> UIResources
        {
            get
            {
                if (_cachedUiResources == null)
                {
                    var embeddedResources = _assembly.GetManifestResourceNames();
                    _cachedUiResources = ParseEmbeddedResources(embeddedResources);
                }
                return _cachedUiResources;
            }
        }

        private IReadOnlyCollection<UIResource> ParseEmbeddedResources(string[] embeddedFiles)
        {
            const char SPLIT_SEPARATOR = '.';

            List<UIResource> resourceList = new List<UIResource>();

            foreach (var file in embeddedFiles)
            {
                if(!file.StartsWith(_embeddedResourcePrefix))
                    continue;

                string filePath = file.Substring(_embeddedResourcePrefix.Length);
                var segments = filePath.Split(SPLIT_SEPARATOR);

                var assetsPath = string.Empty;
                if (segments.Length > 2)
                {
                    string[] pathSegments = segments.Take(segments.Length - 2).ToArray();
                    assetsPath = string.Join("/", pathSegments) + "/";
                }

                var fileName = segments[segments.Length - 2];
                var extension = segments[segments.Length - 1];

                using (var contentStream = _assembly.GetManifestResourceStream(file))
                {
                    byte[] resourceData;
                    using (var memoryStream = new MemoryStream())
                    {
                        contentStream.CopyTo(memoryStream);
                        resourceData = memoryStream.ToArray();
                    }

                    resourceList.Add(
                        UIResource.Create($"{assetsPath}{fileName}.{extension}", resourceData, ContentType.FromExtension(extension)));
                }
            }

            return resourceList;
        }
    }
}