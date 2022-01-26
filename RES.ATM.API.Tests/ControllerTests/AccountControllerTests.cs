using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RES.ATM.API.Controllers.Account;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.Domain.Account.Model;
using RES.ATM.API.DTO.Account;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RES.ATM.API.Tests.ControllerTests
{
    public class AccountControllerTests
    {
        #region GetBalance Test
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AccountController_GetBalance_No_AccountId_ReturnsValidationError(int accountId)
        {
            // Arrange
            var controller = new AccountController(new Mock<IAccountService>().Object);

            // Act 
            var actionResult = await controller.GetBalance(accountId);

            // Assert
            ((actionResult.Result as ObjectResult).Value as ValidationProblemDetails).Errors.Should().ContainKey(nameof(accountId));
        }

        [Fact]
        public async Task AccountController_GetBalance_Returns_Balance()
        {
            // Arrange
            var balance = 100M;
            var accountId = 1234;
            var balanceDto = new BalanceDto { AccountId = accountId, Balance = balance };
            var accountService = new Mock<IAccountService>();
            accountService.Setup(m => m.GetBalance(It.IsAny<int>())).ReturnsAsync(balance);
            var controller = new AccountController(accountService.Object);

            // Act 
            var actionResult = await controller.GetBalance(accountId);

            // Assert
            var result = (actionResult.Result as OkObjectResult).Value;
            result.Should().BeEquivalentTo(balanceDto);
            accountService.Verify(m => m.GetBalance(accountId));            
        }
        #endregion

        #region ValidateAccount Tests
        [Fact]
        public async Task AccountController_ValidateAccount_No_AccountNumber_ReturnsValidationError()
        {
            // Arrange
            var validationDto = new AccountValidationDto
            {
                AccountNumber = Guid.Empty,
                PinNumber = 1234
            };
            var controller = new AccountController(new Mock<IAccountService>().Object);

            // Act 
            var actionResult = await controller.ValidateAccount(validationDto);

            // Assert
            ((actionResult.Result as ObjectResult).Value as ValidationProblemDetails).Errors.Should().ContainKey(nameof(validationDto.AccountNumber));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(999)]
        [InlineData(-1)]
        public async Task AccountController_ValidateAccount_No_PinNumber_ReturnsValidationError(int pinNumber)
        {
            // Arrange
            var validationDto = new AccountValidationDto
            {
                AccountNumber = Guid.NewGuid(),
                PinNumber = pinNumber
            };
            var controller = new AccountController(new Mock<IAccountService>().Object);

            // Act 
            var actionResult = await controller.ValidateAccount(validationDto);

            // Assert
            ((actionResult.Result as ObjectResult).Value as ValidationProblemDetails).Errors.Should().ContainKey(nameof(validationDto.PinNumber));
        }

        [Fact]
        public async Task AccountController_ValidateAccount_Returns_Account()
        {
            // Arrange
            var accountNumber = Guid.NewGuid();
            var pinNumber = 1234;

            var account = new Account
            {
                AccountId = 1,
                AccountNumber = accountNumber,
                AccountName = "James Jones",
                Balance = 500,
                HasOverDraft = true,
                OverDraftAmount = 100,
                PinNumber = pinNumber
            };
            var balanceDto = new BalanceDto { AccountId = account.AccountId, Balance = account.WithdrawalBalance };
            var validationDto = new AccountValidationDto
            {
                AccountNumber = Guid.NewGuid(),
                PinNumber = 1234
            };
            var accountService = new Mock<IAccountService>();
            accountService.Setup(m => m.ValidateAccount(validationDto.AccountNumber, validationDto.PinNumber)).ReturnsAsync(account);
            var controller = new AccountController(accountService.Object);

            // Act 
            var actionResult = await controller.ValidateAccount(validationDto);

            // Assert
            var result = (actionResult.Result as OkObjectResult).Value;
            result.Should().BeEquivalentTo(balanceDto);
            accountService.Verify(m => m.ValidateAccount(validationDto.AccountNumber, validationDto.PinNumber));
        }
        #endregion

        #region Withdrawal Tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AccountController_WithDrawal_No_AccountId_ReturnsValidationError(int accountId)
        {
            // Arrange
            var withdrawalDto = new WithdrawalDto
            {
                AccountId = accountId,
                WithdrawalAmount = 123
            };
            var controller = new AccountController(new Mock<IAccountService>().Object);

            // Act 
            var actionResult = await controller.Withdraw(withdrawalDto);

            // Assert
            ((actionResult.Result as ObjectResult).Value as ValidationProblemDetails).Errors.Should().ContainKey(nameof(withdrawalDto.AccountId));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task AccountController_WithDrawal_No_WithdrawalAmount_ReturnsValidationError(decimal withdrawalAmount)
        {
            // Arrange
            var withdrawalDto = new WithdrawalDto
            {
                AccountId = 12345,
                WithdrawalAmount = withdrawalAmount
            };
            var controller = new AccountController(new Mock<IAccountService>().Object);

            // Act 
            var actionResult = await controller.Withdraw(withdrawalDto);

            // Assert
            ((actionResult.Result as ObjectResult).Value as ValidationProblemDetails).Errors.Should().ContainKey(nameof(withdrawalDto.WithdrawalAmount));
        }

        [Fact]
        public async Task AccountController_ValidateAccount_Returns_WithdrawalAmount()
        {
            // Arrange            
            var withdrawalDto = new WithdrawalDto
            {
                AccountId = 12345,
                WithdrawalAmount = 500
            };
            var balanceDto = new BalanceDto { AccountId = withdrawalDto.AccountId, Balance = withdrawalDto.WithdrawalAmount };
            var accountService = new Mock<IAccountService>();
            accountService.Setup(m => m.Withdraw(withdrawalDto.AccountId, withdrawalDto.WithdrawalAmount)).ReturnsAsync(withdrawalDto.WithdrawalAmount);
            var controller = new AccountController(accountService.Object);

            // Act 
            var actionResult = await controller.Withdraw(withdrawalDto);

            // Assert
            var result = (actionResult.Result as OkObjectResult).Value;
            result.Should().BeEquivalentTo(balanceDto);
            accountService.Verify(m => m.Withdraw(withdrawalDto.AccountId, withdrawalDto.WithdrawalAmount));
        }

        #endregion
    }
}
