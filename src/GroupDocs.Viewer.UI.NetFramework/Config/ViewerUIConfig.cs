using System;
using System.Collections.Generic;
using System.IO;
using GroupDocs.Viewer.UI.NetFramework.Core.Extensions;

namespace GroupDocs.Viewer.UI.NetFramework
{
    public class ViewerUIConfig
    {
        /// <summary>
        /// The UI path. The default value is "/viewer"
        /// </summary>
        public string UIPath { get; set; } = "/viewer";

        /// <summary>
        /// HTML document title. The default value is "GroupDocs.Viewer UI".
        /// </summary>
        public string UITitle { get; set; } = "GroupDocs.Viewer UI";

        /// <summary>
        /// The API path or endpoint. The default value is "/viewer-api"
        /// </summary>
        public string ApiEndpoint { get; set; } = "/viewer-api";

        /// <summary>
        /// Options to configure client application
        /// </summary>
        public ClientAppConfig ClientAppConfig { get; set; } = new ClientAppConfig();

        /// <summary>
        /// Custom stylesheets list.
        /// </summary>
        internal ICollection<string> CustomStylesheets { get; } = new List<string>();

        /// <summary>
        /// Adds custom stylesheet path.
        /// </summary>
        /// <param name="path">Custom stylesheet relative or absolute path.</param>
        /// <returns>This class instance.</returns>
        /// <exception cref="Exception">Throws exception when file does not exist.</exception>
        public ViewerUIConfig AddCustomStylesheet(string path)
        {
            string stylesheetPath = path;

            if (!PathExtensions.IsPathFullyQualified(stylesheetPath))
                stylesheetPath = Path.Combine(Environment.CurrentDirectory, path);

            if (!File.Exists(stylesheetPath))
                throw new Exception($"Could not find style sheet at path {stylesheetPath}");

            CustomStylesheets.Add(stylesheetPath);

            return this;
        }
    }
}
