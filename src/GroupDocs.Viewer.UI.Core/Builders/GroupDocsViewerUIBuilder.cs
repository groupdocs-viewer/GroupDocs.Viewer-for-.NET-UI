using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class GroupDocsViewerUIBuilder
    {
        public IServiceCollection Services { get; }

        public GroupDocsViewerUIBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
