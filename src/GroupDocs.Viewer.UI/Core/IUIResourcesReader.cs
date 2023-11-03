using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core
{
    internal interface IUIResourcesReader
    {
        IReadOnlyCollection<UIResource> UIResources { get; }
    }
}