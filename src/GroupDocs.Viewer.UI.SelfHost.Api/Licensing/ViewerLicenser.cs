using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Licensing
{
    internal class ViewerLicenser : IViewerLicenser
    {
        private readonly Config _config;
        private readonly object _lock = new object();
        private bool _licenseSet;

        public ViewerLicenser(IOptions<Config> config)
        {
            _config = config.Value;
        }

        public void SetLicense()
        {
            if (string.IsNullOrEmpty(_config.LicensePath))
                return;

            if (!_licenseSet)
            {
                lock (_lock)
                {
                    if (!_licenseSet)
                    {
                        License license = new License();
                        license.SetLicense(_config.LicensePath);

                        _licenseSet = true;
                    }
                }
            }
        }

    }
}