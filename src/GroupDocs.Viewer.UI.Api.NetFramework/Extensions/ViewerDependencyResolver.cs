using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace GroupDocs.Viewer.UI.Api.NetFramework.Extensions;

public class ViewerDependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _serviceProvider;

    public ViewerDependencyResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDependencyScope BeginScope()
    {
        return this; // Web API does not natively support child scopes
    }

    public object GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return _serviceProvider.GetServices(serviceType);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // Cleanup
    }
}