using ClearBank.DeveloperTest.Services.DataStore;
using ClearBank.DeveloperTest.Services.DataStore.DataFactory;
using ClearBank.DeveloperTest.Services.Payment;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using ClearBank.DeveloperTest.Validation.Bacs;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentTests
    {

        [Fact]
        public void MakePayment_WhenAccountNotFound_ReturnsFailure()
        {
            // arrange
            var mockFactory = new Mock<IAccountDataStoreFactory>();
            var mockDataStore = new Mock<IAccountDataStore>();

            mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns((Account)null);
            mockFactory.Setup(x => x.GetAccountDataStore()).Returns(mockDataStore.Object);

            var validators = new List<IPaymentSchemeValidator>
            {
                new BacsPaymentSchemeValidator()
            };

            var service = new PaymentService(mockFactory.Object, validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);
            
            // assert
            Assert.False(result.Success);
            mockDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

    }
}
