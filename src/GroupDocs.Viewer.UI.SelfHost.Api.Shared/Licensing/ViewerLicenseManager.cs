using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Licensing
{
    public class ViewerLicenseManager : IViewerLicenseManager
    {
        private readonly Config _config;
        private readonly object _lock = new object();
        private bool _licenseSet;

        public ViewerLicenseManager(IOptions<Config> config)
        {
            _config = config.Value;
        }

        public void SetLicense()
        {
            bool evaluationMode = true;

            if (_licenseSet)
                return;

            StringBuilder errors = new StringBuilder();

            if (!string.IsNullOrEmpty(_config.LicensePath))
            {
                evaluationMode = false;

                try
                {
                    SetLicense(_config.LicensePath);
                }
                catch (Exception ex)
                {
                    errors.Append($"Check config license path error {Environment.NewLine}");
                    errors.Append(ex.Message);
                    errors.Append(Environment.NewLine);
                }

                if (_licenseSet)
                {
                    return;
                }
            }

            string licensePath = Environment.GetEnvironmentVariable(
                Keys.LIC_PATH_ENV_VAR_KEY);
            if (!string.IsNullOrEmpty(licensePath))
            {
                try
                {
                    SetLicense(licensePath);
                }
                catch (Exception ex)
                {
                    errors.Append($"Check environment variable {Keys.LIC_PATH_ENV_VAR_KEY} error {Environment.NewLine}");
                    errors.Append(ex.Message);
                    errors.Append(Environment.NewLine);
                }

                if (_licenseSet)
                {
                    return;
                }
            }

            List<string> licFileNames = new List<string>
            {
                Keys.LIC_FILE_DEFAULT_NAME,
                Keys.TEMP_LIC_FILE_DEFAULT_NAME
            };

            foreach (string licFileName in licFileNames)
            {
                string licPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, licFileName);

                try
                {
                    SetLicense(licPath);
                }
                catch (Exception ex)
                {
                    errors.Append($"Check {licPath} error {Environment.NewLine}");
                    errors.Append(ex.Message);
                    errors.Append(Environment.NewLine);
                }

                if (_licenseSet)
                {
                    return;
                }
            }
#if DEBUG
            if (!evaluationMode)
            {
                throw new FileNotFoundException(errors.ToString());
            }
#endif
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