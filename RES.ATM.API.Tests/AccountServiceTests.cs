using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.Domain.Account.Services;
using RES.ATM.API.Domain.Contracts;
using RES.ATM.API.Domain.Account.Model;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RES.ATM.API.Tests
{
    public class AccountServiceTests
    {
        #region ValidateAccount Tests

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000", 9999)]
        [InlineData("A090AD4A-75E7-4CFB-844F-13B4AE84E4DC", 1)]
        [InlineData("A090AD4A-75E7-4CFB-844F-13B4AE84E4DC", 12)]
        [InlineData("A090AD4A-75E7-4CFB-844F-13B4AE84E4DC", 123)]
        public async Task AccountService_ValidateAccount_Parameters_NotProvided_ThrowsException(Guid accountNumber, int pinNumber)
        {
            // Arrange 
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, new Mock<IAccountRepository>().Object);

            // Act
            Func<Task<Account>> action = async () => await service.ValidateAccount(accountNumber, pinNumber);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AccountService_ValidateAccount_AccountExists_Return_True()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            var accountNumber = Guid.NewGuid();
            var pinNumber = new Random().Next(1000, 9999);
            var account = new Account
            {
                AccountId = 15,
                AccountName = "James",
                PinNumber = pinNumber,
                AccountNumber = accountNumber,
                Balance = 500,
                HasOverDraft = true,
                OverDraftAmount = 100
            };
            repository.Setup(m => m.ValidateAccount(accountNumber, pinNumber)).ReturnsAsync(account);
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, repository.Object);

            // Act
            var result = await service.ValidateAccount(accountNumber, pinNumber);

            // Assert
            repository.Verify(m => m.ValidateAccount(accountNumber, pinNumber));
            result.Should().NotBe(default);
            result.AccountNumber.Should().Be(accountNumber);
            result.PinNumber.Should().Be(pinNumber);
        }

        [Fact]
        public async Task AccountService_ValidateAccount_AccountExists_Return_False()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            var accountNumber = Guid.NewGuid();
            var pinNumber = new Random().Next(1000, 9999);
            repository.Setup(m => m.ValidateAccount(accountNumber, pinNumber)).ReturnsAsync((Account)null);
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, repository.Object);

            // Act
            var result = await service.ValidateAccount(accountNumber, pinNumber);

            // Assert
            repository.Verify(m => m.ValidateAccount(accountNumber, pinNumber));
            result.Should().Be(null);
        }

        #endregion

        #region GetBalance Tests

        [Fact]
        public async Task AccountService_GetBalance_No_AccountNumber_ThrowsException()
        {
            // Arrange 
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, new Mock<IAccountRepository>().Object);

            // Act
            Func<Task<decimal>> action = async () => await service.GetBalance(0);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AccountService_GetBalance_AccountId_Not_Exists_ThrowsException()
        {
            // Arrange 
            var accountRepository = new Mock<IAccountRepository>();
            accountRepository.Setup(m => m.GetBalance(It.IsAny<int>())).ReturnsAsync((Account)null);
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, accountRepository.Object);

            // Act
            Func<Task<decimal>> action = async () => await service.GetBalance(It.IsAny<int>());

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AccountService_GetBalance_HasOverDraft_IsTrue_Return_Balance()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            var accountId = 15;
            var account = new Account
            {
                AccountId = accountId,
                AccountName = "James",
                PinNumber = 1234,
                AccountNumber = Guid.Empty,
                Balance = 500,
                HasOverDraft = true,
                OverDraftAmount = 100
            };
            repository.Setup(m => m.GetBalance(accountId)).ReturnsAsync(account);
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, repository.Object);

            // Act
            var result = await service.GetBalance(accountId);

            // Assert
            result.Should().Be(600);
        }

        [Fact]
        public async Task AccountService_GetBalance_HasOverDraft_IsFalse_Return_Balance()
        {
            // Arrange
            var repository = new Mock<IAccountRepository>();
            var accountId = 15;
            var account = new Account
            {
                AccountId = accountId,
                AccountName = "James",
                PinNumber = 1234,
                AccountNumber = Guid.Empty,
                Balance = 500,
                HasOverDraft = false,
                OverDraftAmount = 100
            };
            repository.Setup(m => m.GetBalance(accountId)).ReturnsAsync(account);
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, repository.Object);

            // Act
            var result = await service.GetBalance(accountId);

            // Assert
            result.Should().Be(500);
        }

        #endregion

        #region WithDraw Tests

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]        
        public async Task AccountService_WithDraw_Incorrect_Parameters_ThrowsException(int accountId, decimal amount)
        {
            // Arrange 
            var service = new AccountService(new Mock<ILogger<IAccountService>>().Object, new Mock<IAccountRepository>().Object);

            // Act
            Func<Task<decimal>> action = async () => await service.WithDraw(accountId, amount);

            // Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        public async Task AccountService_WithDraw_Return_Amount()
        {

        }

        #endregion

    }
}
