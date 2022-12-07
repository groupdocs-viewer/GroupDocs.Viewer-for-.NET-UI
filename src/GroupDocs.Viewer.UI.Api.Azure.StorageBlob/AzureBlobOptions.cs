using Azure.Storage.Blobs;

namespace GroupDocs.Viewer.UI.Api.Azure.StorageBlob
{
    public class AzureBlobOptions
    {
        /// <summary>
        /// For more information,
        /// <see href="https://docs.microsoft.com/azure/storage/common/storage-configure-connection-string">
        /// Configure Azure Storage connection strings</see>
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// For more information,
        /// <see href="https://docs.microsoft.com/azure/storage/common/storage-configure-connection-string">
        /// Configure Azure Storage connection strings</see>
        /// </summary>
        public string AccountKey { get; set; }

        /// <summary>
        /// The name of the container in the storage account to reference.
        /// </summary>
		public string ContainerName { get; set; }

        /// <summary>
        /// Optional client options that define the transport pipeline
        /// policies for authentication, retries, etc., that are applied to
        /// every request.
        /// </summary>
		public BlobClientOptions ClientOptions { get; set; } = new BlobClientOptions();
    }
}
