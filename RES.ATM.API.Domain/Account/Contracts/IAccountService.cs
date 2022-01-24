using System;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.Account.Contracts
{
    public interface IAccountService
    {
        Task<Model.Account> ValidateAccount(Guid accountNumber, int pinNumber);
        Task<decimal> GetBalance(int accountId);
        Task<decimal> WithDraw(int accountId, decimal amount);
    }
}
