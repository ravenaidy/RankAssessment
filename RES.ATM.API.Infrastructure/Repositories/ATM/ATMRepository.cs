using RES.ATM.API.Infrastructure.DBConnection.Contracts;
using RES.ATM.API.Infrastructure.Repositories.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RES.ATM.API.Infrastructure.Repositories.ATM
{
    public class ATMRepository : DapperRepositoryBase
    {
        public ATMRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
    }
}
