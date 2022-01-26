using MediatR;

namespace RES.ATM.API.Domain.ATM.Commands
{
    public class WithdrawalCommand : IRequest
    {
        public WithdrawalCommand(decimal withdrawalAmount)
        {
            WithdrawalAmount = withdrawalAmount;
        }
        public decimal WithdrawalAmount { get; set; }
    }
}
