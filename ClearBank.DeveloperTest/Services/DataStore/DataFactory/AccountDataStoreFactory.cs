using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.Configuration;

namespace ClearBank.DeveloperTest.Services.DataStore.DataFactory
{
    public class AccountDataStoreFactory(IConfigurationService configurationService) : IAccountDataStoreFactory
    {
        public IAccountDataStore GetAccountDataStore()
        {
            var dataStoreType = configurationService.GetDataStoreType();

            return dataStoreType == "Backup"
                ? new BackupAccountDataStore() 
                : new AccountDataStore(); 
        }
    }
}
