namespace GroupDocs.Viewer.UI.Api
{
    public class ApiNames
    {
        public const string CONTROLLER_NAME = "viewer";

        /// <summary>
        /// The route value that the viewer controller requires. Only the routes mapped by <c>MapGroupDocsViewerApi</c>
        /// provide it, so other conventional routes of the app, e.g. the default route, can't reach the controller.
        /// Add it, with <see cref="ROUTE_VALUE"/>, to the defaults of your own conventional route to the controller.
        /// </summary>
        public const string ROUTE_VALUE_KEY = "groupdocs-viewer-api";

        /// <summary>
        /// The value of the <see cref="ROUTE_VALUE_KEY"/> route value.
        /// </summary>
        public const string ROUTE_VALUE = "true";

        public const string API_METHOD_LIST_DIR = "list-dir";
        public const string API_METHOD_UPLOAD_FILE = "upload-file";
        public const string API_METHOD_VIEW_DATA = "view-data";
        public const string API_METHOD_CREATE_PAGES = "create-pages";
        public const string API_METHOD_CREATE_PDF = "create-pdf";
        public const string API_METHOD_GET_PAGE = "get-page";
        public const string API_METHOD_GET_THUMB = "get-thumb";
        public const string API_METHOD_GET_PDF = "get-pdf";
        public const string API_METHOD_GET_RESOURCE = "get-resource";
    }
}
