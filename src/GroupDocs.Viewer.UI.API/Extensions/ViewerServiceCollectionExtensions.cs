using GroupDocs.Viewer.UI.Api.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroupDocs.Viewer.UI.Api.Extensions;

public static class ViewerServiceCollectionExtensions
{
    /// <summary>
    /// Adds DI service required by the editor in trial mode.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName"></param>
    /// <returns></returns>
    public static IServiceCollection AddViewerConfiguration(
        this IServiceCollection services, IConfiguration configuration, string sectionName = "ViewerConfiguration")
    {
        var configSection = configuration.GetSection(sectionName);
        if (!configSection.Exists())
        {
            configSection.Bind(ViewerConfiguration.Instance);
        }
        services.Configure<ViewerConfiguration>(configSection);
        return services;
    }

}