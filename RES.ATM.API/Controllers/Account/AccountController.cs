using Microsoft.AspNetCore.Mvc;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.DTO.Account;
using RES.ATM.API.Shared.Helpers;
using System;
using System.Threading.Tasks;

namespace RES.ATM.API.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentException(nameof(accountService));
        }

        [HttpGet("getbalance")]
        public async Task<ActionResult<BalanceDto>> GetBalance(int accountId)
        {
            if (accountId <= 0)
                ModelState.AddModelError(nameof(accountId), $"Incorrect {nameof(accountId)} were provided");

            if (!ModelState.IsValid)
                return ValidationProblem();
            // Return current balance of account. TODO: Maybe add mapping
            return Ok(new BalanceDto { AccountId = accountId, Balance = await _accountService.GetBalance(accountId) });
        }

        [HttpPost("validateAccount")]
        public async Task<ActionResult<BalanceDto>> ValidateAccount(AccountValidationDto accountValidationDto)
        {
            if (accountValidationDto.AccountNumber == default)
                ModelState.AddModelError(nameof(accountValidationDto.AccountNumber), $"Incorrect {nameof(accountValidationDto.AccountNumber)} were provided");
            if (!accountValidationDto.PinNumber.IsValidPin())
                ModelState.AddModelError(nameof(accountValidationDto.PinNumber), $"Incorrect {nameof(accountValidationDto.PinNumber)} were provided");
            if (!ModelState.IsValid)
                return ValidationProblem();
            
            var account = await _accountService.ValidateAccount(accountValidationDto.AccountNumber, accountValidationDto.PinNumber);
            // Return current balance of account. TODO: Maybe add mapping
            return Ok(new BalanceDto { AccountId = account.AccountId, Balance = account.WithdrawalBalance });
        }

        [HttpPost("withdrawal")]
        public async Task<ActionResult<BalanceDto>> Withdraw(WithdrawalDto withdrawalDto)
        {
            if (withdrawalDto.AccountId <= 0)
                ModelState.AddModelError(nameof(withdrawalDto.AccountId), $"Incorrect {nameof(withdrawalDto.AccountId)} were provided");
            if (withdrawalDto.WithdrawalAmount <= 0)
                ModelState.AddModelError(nameof(withdrawalDto.WithdrawalAmount), $"Incorrect {nameof(withdrawalDto.WithdrawalAmount)} were provided");
            if (!ModelState.IsValid)
                return ValidationProblem();

            // Return current balance of account. TODO: Maybe add mapping
            return Ok(new BalanceDto { AccountId = withdrawalDto.AccountId, Balance = await _accountService.Withdraw(withdrawalDto.AccountId, withdrawalDto.WithdrawalAmount) });
        }
    }
}
