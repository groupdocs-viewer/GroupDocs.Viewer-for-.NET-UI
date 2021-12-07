namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.Configuration
{
    public class Config
    {
        internal string ApiEndpoint = "https://api.groupdocs.cloud/v2.0/";
        internal string ClientId = string.Empty;
        internal string ClientSecret = string.Empty;
        internal string StorageName = string.Empty;

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
    }
}
