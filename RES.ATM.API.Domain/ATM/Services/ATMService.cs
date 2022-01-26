using RES.ATM.API.Domain.ATM.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.ATM.Services
{
    public class ATMService : IATMService
    {
        public ATMService(decimal amount)
        {
            ATMAmount = amount;
        }

        public decimal ATMAmount { get; set; }
    }
}
