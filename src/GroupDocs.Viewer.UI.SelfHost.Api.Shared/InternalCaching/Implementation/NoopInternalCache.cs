using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching
{
    public class NoopInternalCache : IInternalCache
    {
        public bool TryGet(FileCredentials fileCredentials, out Viewer viewer)
        {
            viewer = null;
            return false;
        }

        public void Set(FileCredentials fileCredentials, Viewer entry) { }

        public void Remove(FileCredentials fileCredentials) { }
    }
}