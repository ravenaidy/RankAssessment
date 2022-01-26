using System;

namespace RES.ATM.API.Shared.Exceptions
{
    public class ATMValidationException : Exception
    {
        public ATMValidationException(string errorCode) : base(errorCode)
        {

        }
    }
}
