using System;

namespace RES.ATM.API.DTO.Account
{
    public class AccountDto
    {
        public int AccountId { get; set; }

        public Guid AccountNumber { get; set; }

        public string AccountName { get; set; }        

        public decimal Balance { get; set; }
        
    }
}
