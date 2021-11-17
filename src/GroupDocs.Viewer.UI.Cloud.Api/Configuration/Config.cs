using System;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Core;

namespace GroupDocs.Viewer.UI.Cloud.Api.Configuration
{
    public class Config
    {
        internal string ApiEndpoint = "https://api.groupdocs.cloud/v2.0/";
        internal string ClientId = string.Empty;
        internal string ClientSecret = string.Empty;
        internal string StorageName = string.Empty;
        internal bool SaveOutput = false;
        internal bool DeleteOutput => !SaveOutput;
        internal string OutputFolderPath = "viewer";
        internal ViewerType ViewerType = ViewerType.HtmlWithEmbeddedResources;
        internal Action<HtmlOptions> HtmlViewOptionsSetupAction = options => { };
        internal Action<ImageOptions> PngViewOptionsSetupAction = options => { };
        internal Action<ImageOptions> JpgViewOptionsSetupAction = options => { };
        internal Action<PdfOptions> PdfViewOptionsSetupAction = options => { };

        /// <summary>
        /// Setup the API endpoint. By default it is "https://api-qa.groupdocs.cloud/v2.0/".
        /// </summary>
        /// <param name="apiEndpoint">GroupDocs.Viewer Cloud API endpoint.</param>
        /// <returns>This instance</returns>
        public Config SetApiEndpoint(string apiEndpoint)
        {
            ApiEndpoint = apiEndpoint;
            return this;
        }

        /// <summary>
        /// The client ID, get it at https://dashboard.groupdocs.cloud/applications
        /// </summary>
        /// <param name="clientId">The client ID</param>
        /// <returns>This instance</returns>
        public Config SetClientId(string clientId)
        {
            ClientId = clientId;
            return this;
        }

        /// <summary>
        /// The client secret, get it at https://dashboard.groupdocs.cloud/applications
        /// </summary>
        /// <param name="clientSecret">The secret</param>
        /// <returns>This instance</returns>
        public Config SetClientSecret(string clientSecret)
        {
            ClientSecret = clientSecret;
            return this;
        }

        /// <summary>
        /// Set storage name, you can find storage list at https://dashboard.groupdocs.cloud/storages.
        /// When not set default storage is used.
        /// </summary>
        /// <param name="storageName">The storage name</param>
        /// <returns>This instance</returns>
        public Config SetStorageName(string storageName)
        {
            StorageName = storageName;
            return this;
        }

        /// <summary>
        /// Set this to <value>true</value> to save output in the cloud storage.
        /// By default output is saved in "viewer" folder.
        /// </summary>
        /// <param name="saveOutput">Save output to cloud storage or not.</param>
        /// <returns>This instance</returns>
        public Config SetSaveOutput(bool saveOutput)
        {
            SaveOutput = saveOutput;
            return this;
        }

        /// <summary>
        /// Set the folder path where to store the output.
        /// By default output is saved in "viewer" folder.
        /// </summary>
        /// <param name="outputFolderPath">The output folder path, "viewer" is default.</param>
        /// <returns>This instance</returns>
        public Config SetOutputFolderPath(string outputFolderPath)
        {
            OutputFolderPath = outputFolderPath;
            return this;
        }

        /// <summary>
        /// Set viewer type, e.g. HTML, PNG, JPG
        /// </summary>
        /// <param name="viewerType"></param>
        /// <returns></returns>
        public Config SetViewerType(ViewerType viewerType)
        {
            ViewerType = viewerType;
            return this;
        }

        /// <summary>
        /// Setup HTML rendering options
        /// </summary>
        /// <param name="setupOptions">Setup options action delegate</param>
        /// <returns>This instance</returns>
        public Config SetupHtmlViewOptions(Action<HtmlOptions> setupOptions)
        {
            if (setupOptions != null)
                HtmlViewOptionsSetupAction = setupOptions;
            
            return this;
        }

        /// <summary>
        /// Setup PNG rendering options
        /// </summary>
        /// <param name="setupOptions">Setup options action delegate</param>
        /// <returns>This instance</returns>
        public Config SetupPngViewOptions(Action<ImageOptions> setupOptions)
        {
            if (setupOptions != null)
                PngViewOptionsSetupAction = setupOptions;

            return this;
        }

        /// <summary>
        /// Setup JPG rendering options
        /// </summary>
        /// <param name="setupOptions">Setup options action delegate</param>
        /// <returns>This instance</returns>
        public Config SetupJpgViewOptions(Action<ImageOptions> setupOptions)
        {
            if (setupOptions != null)
                JpgViewOptionsSetupAction = setupOptions;

            return this;
        }

        /// <summary>
        /// Setup PDF rendering options
        /// </summary>
        /// <param name="setupOptions">Setup options action delegate</param>
        /// <returns>This instance</returns>
        public Config SetupPdfViewOptions(Action<PdfOptions> setupOptions)
        {
            if (setupOptions != null)
                PdfViewOptionsSetupAction = setupOptions;

            return this;
        }
    }
}
