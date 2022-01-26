using System;

namespace RES.ATM.API.Shared.Exceptions
{
    public class ATMNoCashException : Exception
    {
        public ATMNoCashException(string errorCode) : base(errorCode)
        {

        }
    }
}
