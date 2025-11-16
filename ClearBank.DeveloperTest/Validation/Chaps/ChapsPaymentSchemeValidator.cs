using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation.Chaps
{
    public class ChapsPaymentSchemeValidator : IPaymentSchemeValidator
    {
        public PaymentScheme Scheme => PaymentScheme.Chaps;

        public bool IsPaymentAllowed(Account account, MakePaymentRequest request)
        {
            if (account is null)
                return false;
            
            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps))
                return false;

            if (account.Status != AccountStatus.Live)
                return false;
            
            return true;
        }
    }
}
