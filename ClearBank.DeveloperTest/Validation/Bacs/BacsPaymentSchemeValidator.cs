using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation.Bacs
{
    public class BacsPaymentSchemeValidator : IPaymentSchemeValidator
    {
        public PaymentScheme Scheme => PaymentScheme.Bacs;

        public bool IsPaymentAllowed(Account account, MakePaymentRequest request)
        {
            if (account is null)
                return false;

            return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
}
