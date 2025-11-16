using ClearBank.DeveloperTest.Services.DataStore.DataFactory;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.DeveloperTest.Services.Payment
{
    public class PaymentService(IAccountDataStoreFactory accountDataStoreFactory, IEnumerable<IPaymentSchemeValidator> validators) : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStore = accountDataStoreFactory.GetAccountDataStore();
            var account = dataStore.GetAccount(request.DebtorAccountNumber);

            var validator = validators.FirstOrDefault(v => v.Scheme == request.PaymentScheme);
            if (validator is null)
            {
                return new MakePaymentResult
                {
                    Success = false
                };
            }
            
            var isValid = validator.IsPaymentAllowed(account, request);
            if (!isValid)
            {
                return new MakePaymentResult
                {
                    Success = false
                };
            }

            account.Balance -= request.Amount;
            dataStore.UpdateAccount(account);

            return new MakePaymentResult
            {
                Success = true
            };
        }
    }
}
