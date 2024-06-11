using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching
{
    public interface IInternalCache
    {
        bool TryGet(FileCredentials fileCredentials, out Viewer viewer);

        void Set(FileCredentials fileCredentials, Viewer entry);
    }
}