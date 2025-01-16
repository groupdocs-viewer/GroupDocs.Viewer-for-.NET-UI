using System.Collections.Generic;
using GroupDocs.Viewer.UI.Api.NetFramework.Controllers;

namespace GroupDocs.Viewer.UI.Api.NetFramework.Endpoints;

public static class ApiMethodMappings
{
    public static Dictionary<string, string> ApiMethods { get; } = new()
    {
        { ApiNames.API_METHOD_LIST_DIR, nameof(ViewerController.ListDir) },
        { ApiNames.API_METHOD_UPLOAD_FILE, nameof(ViewerController.UploadFile) },
        { ApiNames.API_METHOD_VIEW_DATA, nameof(ViewerController.ViewerData) },
        { ApiNames.API_METHOD_CREATE_PAGES, nameof(ViewerController.CreatePages) },
        { ApiNames.API_METHOD_CREATE_PDF, nameof(ViewerController.CreatePdf) },
        { ApiNames.API_METHOD_GET_PAGE, nameof(ViewerController.GetPage) },
        { ApiNames.API_METHOD_GET_THUMB, nameof(ViewerController.GetThumb) },
        { ApiNames.API_METHOD_GET_PDF, nameof(ViewerController.GetPdf) },
        { ApiNames.API_METHOD_GET_RESOURCE, nameof(ViewerController.GetResource) }
    };
}