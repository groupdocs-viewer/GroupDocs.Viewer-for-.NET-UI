using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GroupDocs.Viewer.UI.Core
{
    internal class UIEmbeddedResourcesReader
        : IUIResourcesReader
    {
        private readonly Assembly _assembly;

        private IReadOnlyCollection<UIResource> _cachedUiResources = null;

        public UIEmbeddedResourcesReader(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
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
                var segments = file.Split(SPLIT_SEPARATOR);
                var fileName = segments[segments.Length - 2];
                var extension = segments[segments.Length - 1];

                using (var contentStream = _assembly.GetManifestResourceStream(file))
                using (var reader = new StreamReader(contentStream))
                {
                    string result = reader.ReadToEnd();

                    resourceList.Add(
                        UIResource.Create($"{fileName}.{extension}", result,
                        ContentType.FromExtension(extension)));
                }
            }

            return resourceList;
        }
    }
}