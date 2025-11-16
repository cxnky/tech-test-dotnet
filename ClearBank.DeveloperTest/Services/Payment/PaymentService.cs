using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            // TODO: Use refactored data store factory here
            var account = new Account();
            var dataStoreType = "";

            var result = new MakePaymentResult();
            result.Success = true;
            
            // TODO: Use refactored validators here

            if (result.Success)
            {
                account.Balance -= request.Amount;

                if (dataStoreType == "Backup")
                {
                    var accountDataStore = new BackupAccountDataStore();
                    accountDataStore.UpdateAccount(account);
                }
                else
                {
                    var accountDataStore = new AccountDataStore();
                    accountDataStore.UpdateAccount(account);
                }
            }

            return result;
        }
    }
}
