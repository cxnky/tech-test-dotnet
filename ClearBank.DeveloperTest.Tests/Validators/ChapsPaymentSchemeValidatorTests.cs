using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation.Chaps;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Validators
{
    public class ChapsPaymentSchemeValidatorTests
    {
        private readonly ChapsPaymentSchemeValidator _validator;

        public ChapsPaymentSchemeValidatorTests()
        {
            _validator = new ChapsPaymentSchemeValidator();
        }

        [Fact]
        public void IsPaymentAllowed_WhenAccountExistsAndChapsAllowedAndStatusIsLive_ReturnsTrue()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 500m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsPaymentAllowed_WithAnyBalance_WhenStatusIsLive_AllowsPayment()
        {
            // Arrange - Chaps doesn't check balance
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 0m, // Zero balance
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 1000m,
                PaymentScheme = PaymentScheme.Chaps
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
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = _validator.IsPaymentAllowed(null, request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPaymentAllowed_WhenChapsNotAllowed_ReturnsFalse()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, // Chaps NOT allowed
                Status = AccountStatus.Live
            };

            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPaymentAllowed_WhenStatusIsDisabled_ReturnsFalse()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };

            var request = new MakePaymentRequest
            {
                Amount = 100m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // Act
            var result = _validator.IsPaymentAllowed(account, request);

            // Assert
            Assert.False(result);
        }
    }
}
