using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RES.ATM.API.Domain.ATM.Contracts
{
    public interface IATMService
    {
        decimal ATMAmount { get; }
    }
}
