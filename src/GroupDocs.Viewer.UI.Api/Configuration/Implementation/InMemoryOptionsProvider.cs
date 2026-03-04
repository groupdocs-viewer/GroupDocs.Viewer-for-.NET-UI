namespace GroupDocs.Viewer.UI.Api.Configuration.Implementation
{
    public class InMemoryOptionsProvider : IOptionsProvider
    {
        private Options _options;

        public Options GetOptions()
        {
            return _options;
        }

        public void SetOptions(Options options)
        {
            _options = options;
        }
    }
}
