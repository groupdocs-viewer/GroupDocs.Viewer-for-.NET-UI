using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public class GroupDocsViewerUIApiBuilder
    {
        public IServiceCollection Services { get; }

        public GroupDocsViewerUIApiBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
