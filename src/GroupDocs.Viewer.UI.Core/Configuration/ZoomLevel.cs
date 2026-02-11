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
        /// Represents the 60% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent60 = new ZoomLevel("60%");

        /// <summary>
        /// Represents the 70% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent70 = new ZoomLevel("70%");

        /// <summary>
        /// Represents the 75% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent75 = new ZoomLevel("75%");

        /// <summary>
        /// Represents the 80% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent80 = new ZoomLevel("80%");

        /// <summary>
        /// Represents the 90% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent90 = new ZoomLevel("90%");

        /// <summary>
        /// Represents the 100% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent100 = new ZoomLevel("100%");

        /// <summary>
        /// Represents the 125% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent125 = new ZoomLevel("125%");

        /// <summary>
        /// Represents the 150% zoom level.
        /// </summary>
        public static readonly ZoomLevel Percent150 = new ZoomLevel("150%");

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