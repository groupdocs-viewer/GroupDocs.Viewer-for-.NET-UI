using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public abstract class Page
    {
        private readonly List<PageResource> _resources = new();

        protected Page(int pageNumber, byte[] data)
        {
            PageNumber = pageNumber;
            Data = data;
        }

        protected Page(int pageNumber, byte[] data, IEnumerable<PageResource> resources)
        {
            PageNumber = pageNumber;
            Data = data;
            _resources.AddRange(resources);
        }

        public IEnumerable<PageResource> Resources => _resources;

        public int PageNumber { get; }

        public byte[] Data { get; }

        public abstract string GetContent();

        public void AddResource(PageResource pageResource)
        {
            _resources.Add(pageResource);
        }

        public PageResource GetResource(string resourceName) =>
            _resources.First(resource =>
                resource.ResourceName.Equals(resourceName, StringComparison.InvariantCulture));
    }
} 