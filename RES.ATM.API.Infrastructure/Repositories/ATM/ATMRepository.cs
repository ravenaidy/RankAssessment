using RES.ATM.API.Infrastructure.DBConnection.Contracts;
using RES.ATM.API.Infrastructure.Repositories.Dapper;

namespace RES.ATM.API.Infrastructure.Repositories.ATM
{
    public class ATMRepository : DapperRepositoryBase
    {
        public ATMRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
    }
}
