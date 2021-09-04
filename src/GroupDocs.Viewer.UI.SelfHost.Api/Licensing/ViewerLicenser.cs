using System;
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
            if (_licenseSet)
                return;

            if (!string.IsNullOrEmpty(_config.LicensePath))
                SetLicense(_config.LicensePath);

            string licensePath = Environment.GetEnvironmentVariable("GROUPDOCS_LIC_PATH");
            if (!string.IsNullOrEmpty(licensePath))
                SetLicense(licensePath);
        }

        private void SetLicense(string licensePath)
        {
            lock (_lock)
            {
                if (!_licenseSet)
                {
                    License license = new License();
                    license.SetLicense(licensePath);

                    _licenseSet = true;
                }
            }
        }
    }
}