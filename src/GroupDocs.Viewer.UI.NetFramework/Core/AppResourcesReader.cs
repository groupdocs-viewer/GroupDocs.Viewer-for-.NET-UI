using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GroupDocs.Viewer.UI.NetFramework.Core
{
    internal class AppResourcesReader
    {
        private readonly Assembly _assembly;
        private readonly string _embeddedResourcePrefix;

        private ICollection<UIResource> _cachedUiResources = null;

        public AppResourcesReader()
        {
            _assembly = this.GetType().Assembly;
            _embeddedResourcePrefix = $"{_assembly.GetName().Name}.App.";
        }

        public ICollection<UIResource> UIResources
        {
            get
            {
                if (_cachedUiResources == null)
                {
                    string[] embeddedResources = _assembly.GetManifestResourceNames();
                    _cachedUiResources = ParseEmbeddedResources(embeddedResources);
                }
                return _cachedUiResources;
            }
        }

        private ICollection<UIResource> ParseEmbeddedResources(string[] embeddedFiles)
        {
            const char SPLIT_SEPARATOR = '.';

            List<UIResource> resourceList = new List<UIResource>();

            foreach (string file in embeddedFiles)
            {
                if(!file.StartsWith(_embeddedResourcePrefix))
                    continue;

                string filePath = file.Substring(_embeddedResourcePrefix.Length);
                string[] segments = filePath.Split(SPLIT_SEPARATOR);

                string assetsPath = string.Empty;
                if (segments.Length > 2)
                {
                    string[] pathSegments = new string[segments.Length - 2];
                    for (int i = 0; i < segments.Length - 2; i++)
                    {
                        pathSegments[i] = segments[i];
                    }
                    assetsPath = string.Join("/", pathSegments) + "/";
                }

                string fileName = segments[segments.Length - 2];
                string extension = segments[segments.Length - 1];

                using (Stream contentStream = _assembly.GetManifestResourceStream(file))
                {
                    byte[] resourceData;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, bytesRead);
                        }
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