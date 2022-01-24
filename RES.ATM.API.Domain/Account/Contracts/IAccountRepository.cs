using System;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.Contracts
{
    public interface IAccountRepository
    {
        Task<Account.Model.Account> ValidateAccount(Guid accountNumber, int pinNumber);

        Task<Account.Model.Account> GetBalance(int accountId);

    }
}
