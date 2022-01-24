using RES.ATM.API.Infrastructure.DBConnection.Contracts;
using System;
using System.Data;

namespace RES.ATM.API.Infrastructure.DBConnection
{
    public class ConnectionFactory<TConnection> : IConnectionFactory where TConnection : IDbConnection, new()
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentException(nameof(connectionString));
        }

        public IDbConnection CreateOpenConnection()
        {
            var connection = new TConnection { ConnectionString = _connectionString };

            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
            }
            catch
            {
                throw new Exception("Error when opening connection to Database");
            }
            return connection;
        }
    }
}
