using System;

namespace RES.ATM.API.Domain.Account.Model
{
    public class Account
    {
        public int AccountId { get; set; }

        public Guid AccountNumber { get; set; }

        public string AccountName { get; set; }

        public int PinNumber { get; set; }

        
        public bool HasOverDraft { get; set; }

        public decimal Balance { get; set; }

        public decimal OverDraftAmount { get; set; }

        // Returns Balance including overdraft if available
        public decimal WithdrawalBalance 
        {
            get
            {
                return HasOverDraft ? Balance + OverDraftAmount : Balance;
            } 
        }
    }
}
