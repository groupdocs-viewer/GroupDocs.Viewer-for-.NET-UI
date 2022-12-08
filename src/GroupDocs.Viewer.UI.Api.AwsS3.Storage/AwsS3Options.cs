using Amazon.S3;

namespace GroupDocs.Viewer.UI.Api.AwsS3.Storage
{
    public class AwsS3Options
    {
        /// <summary>
        /// The region constant that determines the endpoint to use.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Access Key. Ignore in case credentials are set globally.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Secret Key. Ignore in case credentials are set globally.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// S3 Config.
        /// </summary>
        public AmazonS3Config S3Config { get; set; } = new AmazonS3Config();

        /// <summary>
        /// S3 bucket name
        /// </summary>
        public string BucketName { get; set; }
    }
}