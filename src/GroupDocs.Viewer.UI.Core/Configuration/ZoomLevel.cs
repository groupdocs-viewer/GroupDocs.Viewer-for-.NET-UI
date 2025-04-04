namespace GroupDocs.Viewer.UI.Core.Configuration
{
    /// <summary>
    /// Represents predefined UI zoom levels.
    /// </summary>
    public class ZoomLevel
    {
        /// <summary>
        /// Represents the "Fit Width" zoom level.
        /// </summary>
        public static readonly ZoomLevel FitWidth = new ZoomLevel("Fit Width");

        /// <summary>
        /// Represents the "Fit Height" zoom level.
        /// </summary>
        public static readonly ZoomLevel FitHeight = new ZoomLevel("Fit Height");

        /// <summary>
        /// Represents the 25% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent25 = new ZoomLevel("25%");

        /// <summary>
        /// Represents the 50% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent50 = new ZoomLevel("50%");

        /// <summary>
        /// Represents the 100% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent100 = new ZoomLevel("100%");

        /// <summary>
        /// Represents the 200% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent200 = new ZoomLevel("200%");

        /// <summary>
        /// Represents the 300% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent300 = new ZoomLevel("300%");

        /// <summary>
        /// The value representing the zoom level.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomLevel"/> class.
        /// </summary>
        /// <param name="value">The string representation of the zoom level.</param>
        private ZoomLevel(string value)
        {
            Value = value;
        }
    }

}