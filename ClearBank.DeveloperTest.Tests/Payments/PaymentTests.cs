using ClearBank.DeveloperTest.Services.DataStore;
using ClearBank.DeveloperTest.Services.DataStore.DataFactory;
using ClearBank.DeveloperTest.Services.Payment;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using ClearBank.DeveloperTest.Validation.Bacs;
using ClearBank.DeveloperTest.Validation.Chaps;
using ClearBank.DeveloperTest.Validation.FasterPayments;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Payments
{
    public class PaymentTests
    {
        private readonly Mock<IAccountDataStoreFactory> _mockFactory;
        private readonly Mock<IAccountDataStore> _mockDataStore;
        private readonly List<IPaymentSchemeValidator> _validators;

        public PaymentTests()
        {
            _mockFactory = new Mock<IAccountDataStoreFactory>();
            _mockDataStore = new Mock<IAccountDataStore>();

            _validators =
            [
                new BacsPaymentSchemeValidator(),
                new FasterPaymentsSchemeValidator(),
                new ChapsPaymentSchemeValidator()
            ];
        }

        #region success tests

        [Fact]
        public void MakePayment_WhenBacsPaymentIsValid_ReturnsSuccessAndUpdatesAccount()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                CreditorAccountNumber = "456",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(900m, account.Balance);

            _mockDataStore.Verify(x => x.GetAccount("123"), Times.Once);
            _mockDataStore.Verify(x => x.UpdateAccount(It.Is<Account>(a => a.Balance == 900m)), Times.Once);

        }

        [Fact]
        public void MakePayment_WhenFasterPaymentsPaymentIsValid_ReturnsSuccessAndUpdatesAccount()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                CreditorAccountNumber = "456",
                Amount = 250.50m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(749.50m, account.Balance);
            
            _mockDataStore.Verify(x => x.GetAccount("123"), Times.Once);
            _mockDataStore.Verify(x => x.UpdateAccount(It.Is<Account>(a => a.Balance == 749.50m)), Times.Once);
        }

        [Fact]
        public void MakePayment_WhenChapsPaymentIsValid_ReturnsSuccessAndUpdatesAccount()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                CreditorAccountNumber = "456",
                Amount = 500m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(500m, account.Balance);
            
            _mockDataStore.Verify(x => x.GetAccount("123"), Times.Once);
            _mockDataStore.Verify(x => x.UpdateAccount(It.Is<Account>(a => a.Balance == 500m)), Times.Once);
        }

        [Fact]
        public void MakePayment_WhenSuccessful_DeductsCorrectAmount()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 333.33m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(666.67m, account.Balance, 2);
        }

        [Fact]
        public void MakePayment_WhenSuccessful_CallsGetAccountWithCorrectAccountNumber()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "DEBTOR123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount("DEBTOR123")).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "DEBTOR123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            service.MakePayment(request);

            // assert
            _mockDataStore.Verify(x => x.GetAccount("DEBTOR123"), Times.Once);
        }

        [Fact]
        public void MakePayment_WhenSuccessful_UpdatesAccountWithModifiedBalance()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 750m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            service.MakePayment(request);

            // assert
            _mockDataStore.Verify(x => x.UpdateAccount(
                It.Is<Account>(a => a.Balance == 250m && a.AccountNumber == "123")),
                Times.Once);
        }

        [Fact]
        public void MakePayment_WhenSuccessful_ReturnsSuccessTrue()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
        }

        #endregion

        #region failure scenarios

        [Fact]
        public void MakePayment_WhenAccountNotFound_ReturnsFailure()
        {
            // arrange
            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns((Account)null);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);
            
            // assert
            Assert.False(result.Success);
            _mockDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void MakePayment_WhenValidatorNotFound_ReturnsFailure()
        {
            // arrange
            var limitedValidators = new List<IPaymentSchemeValidator>
            {
                new BacsPaymentSchemeValidator()
            };

            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, limitedValidators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
            _mockDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void MakePayment_WhenValidationFails_DoesNotUpdateAccount()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, // Bacs NOT allowed
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
            _mockDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public void MakePayment_WhenValidationFails_DoesNotDeductBalance()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 50m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m, // More than balance
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
            Assert.Equal(50m, account.Balance); 
        }

        [Fact]
        public void MakePayment_WhenBacsNotAllowed_ReturnsFailure()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, 
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_WhenFasterPaymentsNotAllowed_ReturnsFailure()
        {
            // Arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_WhenChapsNotAllowed_ReturnsFailure()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, 
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_WhenFasterPaymentsInsufficientBalance_ReturnsFailure()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 99.99m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_WhenChapsAccountNotLive_ReturnsFailure()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Disabled
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 100m,
                PaymentScheme = PaymentScheme.Chaps
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.False(result.Success);
        }

        #endregion

        #region deciaml precision tests

        [Fact]
        public void MakePayment_WithDecimalAmount_PreciseToTwoDecimals_ProcessesCorrectly()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000.00m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 123.45m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(876.55m, account.Balance);
        }

        [Fact]
        public void MakePayment_WithPreciseDecimalCalculation_HandlesCorrectly()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 1000.99m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 0.01m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(1000.98m, account.Balance);
        }

        [Fact]
        public void MakePayment_WithComplexDecimalPrecision_ProcessesCorrectly()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 9999.99m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 3333.33m,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            Assert.Equal(6666.66m, account.Balance);
        }

        // probably overkill, but ¯\_(ツ)_/¯ - at least if this passes, we know that decimal handling is solid
        [Fact]
        public void MakePayment_WithMaximumDecimalValue_HandlesCorrectly()
        {
            // arrange
            var account = new Account
            {
                AccountNumber = "123",
                Balance = 9999999999999999999999999999.99m,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs,
                Status = AccountStatus.Live
            };

            _mockDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(account);
            _mockFactory.Setup(x => x.GetAccountDataStore()).Returns(_mockDataStore.Object);

            var service = new PaymentService(_mockFactory.Object, _validators);
            var request = new MakePaymentRequest
            {
                DebtorAccountNumber = "123",
                Amount = 1234567890123456789012345678.90m,
                PaymentScheme = PaymentScheme.Bacs
            };

            // act
            var result = service.MakePayment(request);

            // assert
            Assert.True(result.Success);
            
            var expectedBalance = 9999999999999999999999999999.99m - 1234567890123456789012345678.90m;
            Assert.Equal(expectedBalance, account.Balance);
        }

        #endregion

    }
}
