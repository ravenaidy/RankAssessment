using RES.ATM.API.Domain.Contracts;
using RES.ATM.API.Infrastructure.DBConnection.Contracts;
using RES.ATM.API.Infrastructure.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var account = new { AccountNumber = accountId };
            var sql = "SELECT 1 FROM Account WHERE AccountNumber=@AccountNumber";
            return await QuerySingleAsync<Domain.Account.Model.Account>(sql, account);
        }

        public async Task<Domain.Account.Model.Account> ValidateAccount(Guid accountNumber, int pinNumber)
        {
            var account = new {AccountNumber = accountNumber, PinNumber = pinNumber};
            var sql = "SELECT 1 FROM Account WHERE AccountNumber=@AccountNumber AND PinNumber=@PinNumber";
            return await QuerySingleAsync<Domain.Account.Model.Account>(sql, account);
        }
    }
}
