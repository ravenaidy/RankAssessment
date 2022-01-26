using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RES.ATM.API.Controllers.Account;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.Domain.Contracts;
using RES.ATM.API.DTO.Account;
using RES.ATM.API.Shared.Constants;
using RES.ATM.API.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace RES.ATM.API.Tests.IntegrationTests
{
    public class ATMIntegrationTest : IClassFixture<CustomWebAppicationFactory<Startup>>
    {
        private readonly CustomWebAppicationFactory<Startup> _factory;

        public ATMIntegrationTest(CustomWebAppicationFactory<Startup> factory)
        {
            _factory = factory ?? throw new ArgumentException(nameof(factory));
        }
        [Fact]
        public async Task Intergration_Test_WithDraw_InValid_Account()
        {
            await InitializeDB();

            // Initialize Controller
            var accountService = (IAccountService)_factory.Services.GetService(typeof(IAccountService));
            var controller = new AccountController(accountService);

            var userAccountNumber = new Guid("4667A3C1-5C36-4F9B-BF07-B588624E8606");
            var userPinNumber = 4567;
            
            var validationAccount = new AccountValidationDto { AccountNumber = userAccountNumber, PinNumber = userPinNumber };
            
            try
            {
                // Do an illegal Validate Account call                
                var accountJourney = GetReponseFromActionResult(await controller.ValidateAccount(validationAccount));
            }
            catch (ATMValidationException exc)
            {
                // Check if correct exception was thrown
                exc.Message.Should().Be(ErrorCodes.ACCOUNT_ERR);
            }
        }

        [Fact]
        public async Task Intergration_Test_WithDraw_InValid_Funds_Journey()
        {
            await InitializeDB();

            // Initialize Controller
            var accountService = (IAccountService)_factory.Services.GetService(typeof(IAccountService));
            var controller = new AccountController(accountService);

            var userAccountNumber = new Guid("4667A3C1-5C36-4F9B-BF07-B588624E8606");
            var userPinNumber = 1234;            
            var validationAccount = new AccountValidationDto { AccountNumber = userAccountNumber, PinNumber = userPinNumber };

            // Validate account
            var accountJourney = GetReponseFromActionResult(await controller.ValidateAccount(validationAccount));

            // Assert Details
            accountJourney.Balance.Should().Be(600);
            accountJourney.AccountId.Should().Be(4);

            var withdrawalDto = new WithdrawalDto { AccountId = accountJourney.AccountId, WithdrawalAmount = 1000 };

            try
            {
                // Do an illegal withdrawal
                var withDrawalJourney = await controller.Withdraw(withdrawalDto);
            }
            catch (ATMNoFundsException exc)
            {
                // Check if correct exception was thrown
                exc.Message.Should().Be(ErrorCodes.FUNDS_ERR);
            }

            // Check balance again
            var checkBalance = GetReponseFromActionResult(await controller.GetBalance(accountJourney.AccountId));
            checkBalance.Balance.Should().Be(600);
        }

        [Fact]
        public async Task Intergration_Test_WithDraw_Valid_Funds_Journey()
        {
            await InitializeDB();

            // Initialize Controller
            var accountService = (IAccountService)_factory.Services.GetService(typeof(IAccountService));
            var controller = new AccountController(accountService);

            var userAccountNumber = new Guid("4667A3C1-5C36-4F9B-BF07-B588624E8606");
            var userPinNumber = 1234;
            
            var validationAccount = new AccountValidationDto { AccountNumber = userAccountNumber, PinNumber = userPinNumber };

            // Validate account
            var accountJourney = GetReponseFromActionResult(await controller.ValidateAccount(validationAccount));

            // Assert Details
            accountJourney.Balance.Should().Be(600);
            accountJourney.AccountId.Should().Be(4);

            // Do legal withdrawal and check if funds is correct
            var withdrawalDto = new WithdrawalDto { AccountId = accountJourney.AccountId, WithdrawalAmount = 400 };
            var withDrawalJourney = GetReponseFromActionResult(await controller.Withdraw(withdrawalDto));
            withDrawalJourney.Balance.Should().Be(200);            
        }

        [Fact]
        public async Task Intergration_Test_WithDraw_ATM_No_Funds_Journey()
        {
            await InitializeDB();

            // Initialize Controller
            var accountService = (IAccountService)_factory.Services.GetService(typeof(IAccountService));
            var controller = new AccountController(accountService);

            // User1 withdraws cash and assertions
            var user1AccountNumber = new Guid("C2AD8F2C-31E9-4D2C-81EC-339D83D35FFC");
            var user1PinNumber = 4321;           
            var validationAccount = new AccountValidationDto { AccountNumber = user1AccountNumber, PinNumber = user1PinNumber };
            
            var accountJourney = GetReponseFromActionResult(await controller.ValidateAccount(validationAccount));            
            accountJourney.Balance.Should().Be(400);
            accountJourney.AccountId.Should().Be(5);

            // Do legal withdrawal and check if funds is correct
            decimal withDrawalAmount = 400;
            var withdrawalDto = new WithdrawalDto { AccountId = accountJourney.AccountId, WithdrawalAmount = withDrawalAmount };
            var withDrawalJourney = GetReponseFromActionResult(await controller.Withdraw(withdrawalDto));
            withDrawalJourney.Balance.Should().Be(0);

            // User2 withdraws cash and assertions
            var user2AccountNumber = new Guid("79B2AE88-6DFE-43AB-9B7E-5F1571A4BF5A");
            var user2PinNumber = 4567;
            
            validationAccount = new AccountValidationDto { AccountNumber = user2AccountNumber, PinNumber = user2PinNumber };
            accountJourney = GetReponseFromActionResult(await controller.ValidateAccount(validationAccount));
            accountJourney.Balance.Should().Be(8000);
            accountJourney.AccountId.Should().Be(6);

            // Do legal withdrawal and check if funds is correct
            withDrawalAmount = 7600;
            withdrawalDto = new WithdrawalDto { AccountId = accountJourney.AccountId, WithdrawalAmount = withDrawalAmount };
            withDrawalJourney = GetReponseFromActionResult(await controller.Withdraw(withdrawalDto));
            withDrawalJourney.Balance.Should().Be(400);

            // User2 withdraws cash and assertions
            var user3AccountNumber = new Guid("4667A3C1-5C36-4F9B-BF07-B588624E8606");
            var user3PinNumber = 1234;
            
            validationAccount = new AccountValidationDto { AccountNumber = user3AccountNumber, PinNumber = user3PinNumber };
            accountJourney = GetReponseFromActionResult(await controller.ValidateAccount(validationAccount));
            accountJourney.Balance.Should().Be(600);
            accountJourney.AccountId.Should().Be(4);

            // Do illegal withdrawal and check if funds is correct
            withDrawalAmount = 400;
            withdrawalDto = new WithdrawalDto { AccountId = accountJourney.AccountId, WithdrawalAmount = withDrawalAmount };

            try
            {
                // Do an illegal withdrawal
                var failedResult = await controller.Withdraw(withdrawalDto);
            }
            catch (ATMNoCashException exc)
            {
                // Check if correct exception was thrown
                exc.Message.Should().Be(ErrorCodes.ATM_ERR);
            }
        }

        private static T GetReponseFromActionResult<T>(ActionResult<T> actionResult)
        {
            return (T)(actionResult.Result as OkObjectResult).Value;
        }

        /// <summary>
        /// Resets DB data for Journey Tests
        /// </summary>
        /// <returns></returns>
        private async Task InitializeDB()
        {
            var accountRepository = (IAccountRepository)_factory.Services.GetService(typeof(IAccountRepository));
            await accountRepository.UpdateBalance(4, 500); // Updates User1 account Balance to 500
            await accountRepository.UpdateBalance(5, 400); // Updates User2 account Balance to 500
            await accountRepository.UpdateBalance(6, 7000); // Updates User3 account Balance to 500
        }
    }
}
