using RES.ATM.API.Domain.Contracts;
using RES.ATM.API.Infrastructure.DBConnection.Contracts;
using RES.ATM.API.Infrastructure.Repositories.Dapper;
using System;
using System.Threading.Tasks;

namespace RES.ATM.API.Infrastructure.Repositories.Account
{
    public class AccountRepository : DapperRepositoryBase, IAccountRepository
    {
        public AccountRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<Domain.Account.Model.Account> GetBalance(int accountId)
        {
            var account = new { AccountId = accountId };
            var sql = "SELECT AccountId, AccountNumber, AccountName, PinNumber, HasOverDraft, Balance, OverDraftAmount FROM Account WHERE AccountId=@AccountId";
            return await QueryFirstOrDefaultAsync<Domain.Account.Model.Account>(sql, account);
        }

        public async Task<bool> UpdateBalance(int accountId, decimal newBalance)
        {
            var account = new { AccountId = accountId, Balance = newBalance };
            var sql = "UPDATE Account SET Balance = @Balance WHERE AccountId = @AccountId";
            return await ExecuteAsync(sql, account) > 0;
        }

        public async Task<Domain.Account.Model.Account> ValidateAccount(Guid accountNumber, int pinNumber)
        {
            var account = new { AccountNumber = accountNumber.ToString().ToUpper(), PinNumber = pinNumber};
            var sql = "SELECT AccountId, AccountNumber, AccountName, PinNumber, HasOverDraft, Balance, OverDraftAmount FROM Account WHERE AccountNumber = @AccountNumber";
            return await QueryFirstOrDefaultAsync<Domain.Account.Model.Account>(sql, account);
        }
    }
}
