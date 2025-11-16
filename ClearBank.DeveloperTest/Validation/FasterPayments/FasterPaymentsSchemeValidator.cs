using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation.FasterPayments
{
    public class FasterPaymentsSchemeValidator : IPaymentSchemeValidator
    {
        public PaymentScheme Scheme => PaymentScheme.FasterPayments;

        public bool IsPaymentAllowed(Account account, MakePaymentRequest request)
        {
            if (account is null)
                return false;

            if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments))
                return false;

            if (account.Balance < request.Amount)
                return false;

            return true;
        }
    }
}
