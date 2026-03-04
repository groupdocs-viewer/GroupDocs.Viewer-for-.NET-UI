namespace GroupDocs.Viewer.UI.Api.Configuration
{
    /// <summary>
    /// This interface is used as a workaround to the issue where API does not now about ApiDomain and ApiPath.
    /// See related implementation that uses Singleton to make Options object kind of `static` but accesible as 
    /// a service using service provider.
    /// </summary>
    public interface IOptionsProvider
    {
        Options GetOptions();

        void SetOptions(Options options);
    }
}
