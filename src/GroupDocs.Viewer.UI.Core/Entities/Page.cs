using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public abstract class Page
    {
        private readonly List<PageResource> _resources = new List<PageResource>();

        protected Page(int pageNumber, byte[] pageData)
        {
            PageNumber = pageNumber;
            PageData = pageData;
        }

        protected Page(int pageNumber, byte[] pageData, IEnumerable<PageResource> resources)
        {
            PageNumber = pageNumber;
            PageData = pageData;
            _resources.AddRange(resources);
        }

        public IEnumerable<PageResource> Resources => _resources;

        public int PageNumber { get; }

        public byte[] PageData { get; protected set; }

        public abstract string ContentType { get; }

        public abstract string GetContent();

        public abstract void SetContent(string content);

        public void AddResource(PageResource pageResource)
        {
            _resources.Add(pageResource);
        }

        public PageResource GetResource(string resourceName) =>
            _resources.First(resource =>
                resource.ResourceName.Equals(resourceName, StringComparison.InvariantCulture));
    }
} 