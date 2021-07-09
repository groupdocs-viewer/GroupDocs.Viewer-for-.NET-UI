using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Viewer.UI.Configuration
{
    public class Options
    {
        internal ICollection<string> CustomStylesheets { get; } = new List<string>();
        
        public string UIPath { get; set; } = "/viewer";

        public string UIConfigEndpoint { get; set; } = "/viewer-config";

        public string APIEndpoint { get; set; } = "/viewer-api";

        public Options AddCustomStylesheet(string path)
        {
            string stylesheetPath = path;

            if (!Path.IsPathFullyQualified(stylesheetPath))
                stylesheetPath = Path.Combine(Environment.CurrentDirectory, path);

            if (!File.Exists(stylesheetPath))
                throw new Exception($"Could not find style sheet at path {stylesheetPath}");

            CustomStylesheets.Add(stylesheetPath);

            return this;
        }
    }
}
