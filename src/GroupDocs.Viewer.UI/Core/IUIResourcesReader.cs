using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core
{
    internal interface IUIResourcesReader
    {
        IEnumerable<UIResource> UIResources { get; }
    }
}