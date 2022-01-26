using System;

namespace RES.ATM.API.DTO.Account
{
    public class AccountValidationDto
    {
        public Guid AccountNumber { get; set; }
        public int PinNumber { get; set; }
    }
}
