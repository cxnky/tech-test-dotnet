using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.DataStore
{
    public interface IAccountDataStore
    {
        Account GetAccount(string accountNumber);
        void UpdateAccount(Account account);
    }
}
