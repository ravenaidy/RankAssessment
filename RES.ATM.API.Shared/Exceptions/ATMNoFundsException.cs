using System;

namespace RES.ATM.API.Shared.Exceptions
{
    public class ATMNoFundsException : Exception
    {
        public ATMNoFundsException(string errorCode) : base(errorCode)
        {

        }
    }
}
