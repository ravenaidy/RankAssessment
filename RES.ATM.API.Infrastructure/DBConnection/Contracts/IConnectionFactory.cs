using System.Data;

namespace RES.ATM.API.Infrastructure.DBConnection.Contracts
{
    public interface IConnectionFactory
    {
        IDbConnection CreateOpenConnection();
    }
}
