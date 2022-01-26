using MediatR;
using Microsoft.Extensions.Logging;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.Domain.ATM.Commands;
using RES.ATM.API.Domain.ATM.Contracts;
using RES.ATM.API.Domain.Contracts;
using RES.ATM.API.Shared.Constants;
using RES.ATM.API.Shared.Exceptions;
using RES.ATM.API.Shared.Helpers;
using System;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<IAccountService> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IATMService _atmService;
        private readonly IMediator _mediator;

        public AccountService(
            ILogger<IAccountService> logger,
            IATMService atmService,
            IAccountRepository accountRepository,
            IMediator mediator)
        {            
            _logger = logger ?? throw new ArgumentException(nameof(_logger));
            _accountRepository = accountRepository ?? throw new ArgumentException(nameof(accountRepository));
            _atmService = atmService ?? throw new ArgumentException(nameof(atmService));
            _mediator = mediator ?? throw new ArgumentException(nameof(mediator));
        }

        public async Task<decimal> GetBalance(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentNullException($"{nameof(GetBalance)} parameter {nameof(accountId)} was not/incorrectly specified");

            var account = await _accountRepository.GetBalance(accountId);

            if (account == default)
                throw new ArgumentNullException($"{nameof(GetBalance)} for {nameof(accountId)}: {accountId} does not exist");

            return account.WithdrawalBalance;
        }        

        public async Task<Model.Account> ValidateAccount(Guid accountNumber, int pinNumber)
        {
            if (accountNumber == Guid.Empty)
                throw new ArgumentNullException($"{nameof(ValidateAccount)} parameter {nameof(accountNumber)} was not/incorrectly specified");

            if (!pinNumber.IsValidPin())
                throw new ArgumentNullException($"{nameof(ValidateAccount)} parameter {nameof(pinNumber)} was not/incorrectly specified");

            var account = await _accountRepository.ValidateAccount(accountNumber, pinNumber);

            // If account not found throw exception
            if (account == null)
                throw new ATMValidationException(ErrorCodes.ACCOUNT_ERR);

           return account;
        }

        public async Task<decimal> Withdraw(int accountId, decimal amount)
        {
            if (accountId == default)
                throw new ArgumentNullException($"{nameof(Withdraw)} parameter {nameof(accountId)} was not/incorrectly specified");

            if (amount == default)
                throw new ArgumentNullException($"{nameof(Withdraw)} parameter {nameof(amount)} was not/incorrectly specified");

            // If Amount is greater than ATM Funds throw exception
            if (amount > _atmService.ATMAmount)
                throw new ATMNoCashException(ErrorCodes.ATM_ERR);
                       
            var account = await _accountRepository.GetBalance(accountId);            

            // If Withdrawal is greater than balance throw exception
            if (account.WithdrawalBalance < amount)
                throw new ATMNoFundsException(ErrorCodes.FUNDS_ERR);

            var isBalanceUpdated = await _accountRepository.UpdateBalance(accountId, (account.Balance - amount));

            if (!isBalanceUpdated)
                throw new Exception($"{nameof(Withdraw)} parameter {nameof(amount)} was not/incorrectly specified");

            await _mediator.Send(new WithdrawalCommand(amount));

            // Return latest balance
            return await GetBalance(accountId);
        }      
    }
}
