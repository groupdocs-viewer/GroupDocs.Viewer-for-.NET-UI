using System;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IAsyncLock
    {
        Task<IDisposable> LockAsync(object key);
    }
}