using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.Configuration;
using ClearBank.DeveloperTest.Services.DataStore;
using ClearBank.DeveloperTest.Services.DataStore.DataFactory;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests.Factories
{
    public class AccountDataStoreFactoryTests
    {
        [Fact]
        public void GetAccountDataStore_WhenConfigurationIsBackup_ReturnsBackupAccountDataStore()
        {
            // Arrange
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.Setup(x => x.GetDataStoreType()).Returns("Backup");

            var factory = new AccountDataStoreFactory(mockConfigurationService.Object);

            // Act
            var result = factory.GetAccountDataStore();

            // Assert
            Assert.IsType<BackupAccountDataStore>(result);
            Assert.IsAssignableFrom<IAccountDataStore>(result);
        }

        [Fact]
        public void GetAccountDataStore_WhenConfigurationIsNotBackup_ReturnsAccountDataStore()
        {
            // Arrange
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.Setup(x => x.GetDataStoreType()).Returns("Primary");

            var factory = new AccountDataStoreFactory(mockConfigurationService.Object);

            // Act
            var result = factory.GetAccountDataStore();

            // Assert
            Assert.IsType<AccountDataStore>(result);
            Assert.IsAssignableFrom<IAccountDataStore>(result);
        }

        [Fact]
        public void GetAccountDataStore_WhenConfigurationIsEmpty_ReturnsAccountDataStore()
        {
            // Arrange
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.Setup(x => x.GetDataStoreType()).Returns(string.Empty);

            var factory = new AccountDataStoreFactory(mockConfigurationService.Object);

            // Act
            var result = factory.GetAccountDataStore();

            // Assert
            Assert.IsType<AccountDataStore>(result);
        }

        [Fact]
        public void GetAccountDataStore_WhenConfigurationIsNull_ReturnsAccountDataStore()
        {
            // Arrange
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.Setup(x => x.GetDataStoreType()).Returns((string)null);

            var factory = new AccountDataStoreFactory(mockConfigurationService.Object);

            // Act
            var result = factory.GetAccountDataStore();

            // Assert
            Assert.IsType<AccountDataStore>(result);
        }

        [Fact]
        public void GetAccountDataStore_CallsConfigurationService()
        {
            // Arrange
            var mockConfigurationService = new Mock<IConfigurationService>();
            mockConfigurationService.Setup(x => x.GetDataStoreType()).Returns("Backup");

            var factory = new AccountDataStoreFactory(mockConfigurationService.Object);

            // Act
            factory.GetAccountDataStore();

            // Assert
            mockConfigurationService.Verify(x => x.GetDataStoreType(), Times.Once);
        }
    }
}
