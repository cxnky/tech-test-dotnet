using System.Configuration;

namespace ClearBank.DeveloperTest.Services.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        public string GetDataStoreType()
        {
            return ConfigurationManager.AppSettings["DataStoreType"] ?? string.Empty;
        }
    }
}
