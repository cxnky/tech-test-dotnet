using ClearBank.DeveloperTest.Services.DataStore.DataFactory;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.DeveloperTest.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStoreFactory _accountDataStoreFactory;
        private readonly IEnumerable<IPaymentSchemeValidator> _validators;

        public PaymentService(IAccountDataStoreFactory accountDataStoreFactory, IEnumerable<IPaymentSchemeValidator> validators)
        {
            _accountDataStoreFactory = accountDataStoreFactory;
            _validators = validators;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStore = _accountDataStoreFactory.CreateDataStore();
            var account = dataStore.GetAccount(request.DebtorAccountNumber);

            var validator = _validators.FirstOrDefault(v => v.Scheme == request.PaymentScheme);
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
