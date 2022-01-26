using MediatR;

namespace RES.ATM.API.Domain.ATM.Events
{
    public class WithdrawalCompletedEvent : INotification
    {
        public WithdrawalCompletedEvent(decimal withdrawalAmount)
        {
            WithdrawalAmount = withdrawalAmount;
        }
        public decimal WithdrawalAmount { get; set; }
    }
}
