using Microsoft.Extensions.Logging;
using RES.ATM.API.Domain.Account.Contracts;
using RES.ATM.API.Domain.Contracts;
using System;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.Account.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<IAccountService> _logger;
        private readonly IAccountRepository _accountRepository;
        public AccountService(ILogger<IAccountService> logger, IAccountRepository accountRepository)
        {            
            _logger = logger ?? throw new ArgumentException(nameof(_logger));
            _accountRepository = accountRepository ?? throw new ArgumentException(nameof(accountRepository));
        }

        public async Task<decimal> GetBalance(int accountId)
        {
            if (accountId == default)
                throw new ArgumentNullException($"{nameof(GetBalance)} parameter {nameof(accountId)} was not/incorrectly specified");

            var account = await _accountRepository.GetBalance(accountId);

            if (account == default)
                throw new ArgumentNullException($"{nameof(GetBalance)} for {nameof(accountId)}: {accountId} does not exist");

            return account.HasOverDraft ? account.Balance + account.OverDraftAmount : account.Balance;
        }

        public async Task<Model.Account> ValidateAccount(Guid accountNumber, int pinNumber)
        {
            if (accountNumber == Guid.Empty)
                throw new ArgumentNullException($"{nameof(ValidateAccount)} parameter {nameof(accountNumber)} was not/incorrectly specified");

            if (pinNumber == default || (pinNumber < 1000 || pinNumber > 9999))
                throw new ArgumentNullException($"{nameof(ValidateAccount)} parameter {nameof(pinNumber)} was not/incorrectly specified");

            return await _accountRepository.ValidateAccount(accountNumber, pinNumber);
        }

        public Task<decimal> WithDraw(int accountId, decimal amount)
        {
            if (accountId == default)
                throw new ArgumentNullException($"{nameof(ValidateAccount)} parameter {nameof(accountId)} was not/incorrectly specified");

            if (amount == default)
                throw new ArgumentNullException($"{nameof(ValidateAccount)} parameter {nameof(amount)} was not/incorrectly specified");

            return default;
        }
    }
}
