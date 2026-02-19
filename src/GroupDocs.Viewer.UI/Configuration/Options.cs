using System;
using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Viewer.UI.Configuration
{
    /// <summary>
    /// Configuration options for GroupDocs.Viewer.UI middleware.
    /// Used in <c>MapGroupDocsViewerUI(options => { ... })</c> to configure the UI path,
    /// API endpoint, page title, custom branding, and stylesheets.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// The UI path at which the viewer SPA is served. The default value is "/viewer".
        /// Must start with "/" character.
        /// </summary>
        public string UIPath { get; set; } = "/viewer";

        /// <summary>
        /// HTML document title shown in the browser tab. The default value is "GroupDocs.Viewer UI".
        /// </summary>
        /// <example>
        /// <code>
        /// options.UITitle = "Acme Corp Document Viewer";
        /// </code>
        /// </example>
        public string UITitle { get; set; } = "GroupDocs.Viewer UI";

        /// <summary>
        /// The API path or endpoint that the UI uses to fetch document data.
        /// The default value is "/viewer-api". Must match the path configured in
        /// <c>MapGroupDocsViewerApi(options => { options.ApiPath = "/viewer-api"; })</c>.
        /// </summary>
        public string ApiEndpoint { get; set; } = "/viewer-api";

        /// <summary>
        /// Custom logo image file path. When set, replaces the default brand icon
        /// (<c>assets/ui/logo-image.svg</c>) displayed in the header.
        /// The image is rendered at 26x23 pixels.
        /// </summary>
        internal string CustomLogoImagePath { get; private set; }

        /// <summary>
        /// Custom logo text file path. When set, replaces the default brand text
        /// (<c>assets/ui/logo-text.svg</c>) displayed in the header next to the logo icon.
        /// The image is rendered at 131x15 pixels.
        /// </summary>
        internal string CustomLogoTextPath { get; private set; }

        /// <summary>
        /// Whether the logo image (brand icon) is hidden. When true, an empty SVG is served
        /// in place of the default logo image so no icon is displayed.
        /// </summary>
        internal bool IsLogoImageHidden { get; private set; }

        /// <summary>
        /// Whether the logo text is hidden. When true, an empty SVG is served
        /// in place of the default logo text so no text logo is displayed.
        /// </summary>
        internal bool IsLogoTextHidden { get; private set; }

        /// <summary>
        /// Custom stylesheets injected as <c>&lt;link&gt;</c> tags into the viewer's index page.
        /// Use <see cref="AddCustomStylesheet"/> to add stylesheets.
        /// </summary>
        internal ICollection<string> CustomStylesheets { get; } = new List<string>();

        /// <summary>
        /// Custom scripts injected as <c>&lt;script&gt;</c> tags into the viewer's index page.
        /// Use <see cref="AddCustomScript"/> to add scripts.
        /// </summary>
        internal ICollection<string> CustomScripts { get; } = new List<string>();

        /// <summary>
        /// Adds a custom stylesheet that will be injected into the viewer's index page.
        /// The stylesheet is served at <c>{UIPath}/css/{filename}</c>.
        /// Use this to override CSS variables (e.g. <c>--c-bg-brand</c>) for custom branding.
        /// Multiple stylesheets can be added by calling this method multiple times.
        /// </summary>
        /// <param name="path">Custom stylesheet relative or absolute file path.
        /// Relative paths are resolved from the current working directory.</param>
        /// <returns>This <see cref="Options"/> instance for fluent chaining.</returns>
        /// <exception cref="Exception">Thrown when the file does not exist at the resolved path.</exception>
        /// <example>
        /// <code>
        /// options.AddCustomStylesheet("./Styles/custom-branding.css");
        /// </code>
        /// </example>
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

        /// <summary>
        /// Adds a custom JavaScript file that will be injected into the viewer's index page.
        /// The script is served at <c>{UIPath}/js/{filename}</c>.
        /// Use this to add custom behavior, event handlers, or integrations.
        /// Multiple scripts can be added by calling this method multiple times.
        /// </summary>
        /// <param name="path">Custom script relative or absolute file path.
        /// Relative paths are resolved from the current working directory.</param>
        /// <returns>This <see cref="Options"/> instance for fluent chaining.</returns>
        /// <exception cref="Exception">Thrown when the file does not exist at the resolved path.</exception>
        /// <example>
        /// <code>
        /// options.AddCustomScript("./Scripts/custom-behavior.js");
        /// </code>
        /// </example>
        public Options AddCustomScript(string path)
        {
            string scriptPath = ResolveAndValidatePath(path);
            CustomScripts.Add(scriptPath);
            return this;
        }

        /// <summary>
        /// Sets a custom logo image that replaces the default brand icon (<c>logo-image.svg</c>)
        /// in the viewer header. The image is displayed at 26x23 pixels. SVG format is recommended.
        /// </summary>
        /// <param name="path">Custom logo image relative or absolute file path.
        /// Relative paths are resolved from the current working directory.</param>
        /// <returns>This <see cref="Options"/> instance for fluent chaining.</returns>
        /// <exception cref="Exception">Thrown when the file does not exist at the resolved path.</exception>
        /// <example>
        /// <code>
        /// options.SetLogoImage("./Logos/logo-image.svg");
        /// </code>
        /// </example>
        public Options SetLogoImage(string path)
        {
            CustomLogoImagePath = ResolveAndValidatePath(path);
            return this;
        }

        /// <summary>
        /// Sets a custom logo text image that replaces the default brand text (<c>logo-text.svg</c>)
        /// displayed in the header next to the logo icon. The image is displayed at 131x15 pixels.
        /// SVG format is recommended.
        /// </summary>
        /// <param name="path">Custom logo text image relative or absolute file path.
        /// Relative paths are resolved from the current working directory.</param>
        /// <returns>This <see cref="Options"/> instance for fluent chaining.</returns>
        /// <exception cref="Exception">Thrown when the file does not exist at the resolved path.</exception>
        /// <example>
        /// <code>
        /// options.SetLogoText("./Logos/logo-text.svg");
        /// </code>
        /// </example>
        public Options SetLogoText(string path)
        {
            CustomLogoTextPath = ResolveAndValidatePath(path);
            return this;
        }

        /// <summary>
        /// Hides the logo image (brand icon) in the header by serving an empty SVG.
        /// Use this when you want to display only the logo text, or hide all logos.
        /// Takes precedence over <see cref="SetLogoImage"/> if both are called.
        /// </summary>
        /// <returns>This <see cref="Options"/> instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// // Show only logo text, hide the icon
        /// options.HideLogoImage();
        /// options.SetLogoText("./Logos/logo-text.svg");
        /// </code>
        /// </example>
        public Options HideLogoImage()
        {
            IsLogoImageHidden = true;
            return this;
        }

        /// <summary>
        /// Hides the logo text in the header by serving an empty SVG.
        /// Use this when you want to display only the logo icon, or hide all logos.
        /// Takes precedence over <see cref="SetLogoText"/> if both are called.
        /// </summary>
        /// <returns>This <see cref="Options"/> instance for fluent chaining.</returns>
        /// <example>
        /// <code>
        /// // Show only logo icon, hide the text
        /// options.SetLogoImage("./Logos/logo-image.svg");
        /// options.HideLogoText();
        /// </code>
        /// </example>
        public Options HideLogoText()
        {
            IsLogoTextHidden = true;
            return this;
        }

        private static string ResolveAndValidatePath(string path)
        {
            string resolvedPath = path;

            if (!Path.IsPathFullyQualified(resolvedPath))
                resolvedPath = Path.Combine(Environment.CurrentDirectory, path);

            if (!File.Exists(resolvedPath))
                throw new Exception($"Could not find file at path {resolvedPath}");

            return resolvedPath;
        }
    }
}
