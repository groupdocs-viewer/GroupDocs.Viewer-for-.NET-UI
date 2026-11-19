namespace GroupDocs.Viewer.UI.Core.Configuration
{
    /// <summary>
    /// Represents the UI color themes.
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Follows the light or dark setting of the operating system.
        /// </summary>
        public static readonly Theme Auto = new Theme("auto");

        /// <summary>
        /// Represents the light theme.
        /// </summary>
        public static readonly Theme Light = new Theme("light");

        /// <summary>
        /// Represents the dark theme.
        /// </summary>
        public static readonly Theme Dark = new Theme("dark");

        /// <summary>
        /// The value representing the theme.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Theme"/> class.
        /// </summary>
        /// <param name="value">The string representation of the theme.</param>
        private Theme(string value)
        {
            Value = value;
        }
    }
}
