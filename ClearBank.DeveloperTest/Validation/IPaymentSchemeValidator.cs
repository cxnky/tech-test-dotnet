using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation
{
    public interface IPaymentSchemeValidator
    {
        bool IsPaymentAllowed(Account account, MakePaymentRequest request);
        PaymentScheme Scheme { get; }
    }
}
