using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation.FasterPayments;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Validators
{
    public class FasterPaymentsSchemeValidatorTests
    {
        private readonly FasterPaymentsSchemeValidator _validator;

        public FasterPaymentsSchemeValidatorTests()
        {
            _validator = new FasterPaymentsSchemeValidator();
        }

        [Fact]
        public void IsPaymentAllowed_WhenAccountExistsAndFasterPaymentsAllowedAndBalanceSufficient_ReturnsTrue()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 500m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPaymentAllowed_WhenBalanceExactlyEqualsAmount_ReturnsTrue()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 100m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPaymentAllowed_WhenBalanceIsGreaterThanAmount_ReturnsTrue()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 100.01m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPaymentAllowed_WhenAccountIsNull_ReturnsFalse()
        {
            // Arrange
            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // Act
            var result = _validator.IsPaymentAllowed(null, request);

            // Assert
            Assert.False(result);
        }

    }
}
