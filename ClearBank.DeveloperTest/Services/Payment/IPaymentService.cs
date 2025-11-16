using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Payment
{
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
    }
}
