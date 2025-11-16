using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation.Bacs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Validators
{
    public class BacsPaymentSchemeValidatorTests
    {
        private readonly BacsPaymentSchemeValidator _validator;

        public BacsPaymentSchemeValidatorTests()
        {
            _validator = new BacsPaymentSchemeValidator();
        }

        [Fact]
        public void IsPaymentAllowed_WhenAccountExistsAndBacsIsAllowed_ReturnsTrue()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = _validator.IsPaymentAllowed(account, request);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void IsPaymentAllowed_WhenAccountHasMultiplePaymentSchemes_ReturnsTrue()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.True(result);
        }

    }
}
