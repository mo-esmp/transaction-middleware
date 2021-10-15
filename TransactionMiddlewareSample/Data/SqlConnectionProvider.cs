using Microsoft.Data.SqlClient;
using System.Data;

namespace TransactionMiddlewareSample.Data
{
    public class SqlConnectionProvider
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public SqlConnectionProvider(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public IDbConnection GetDbConnection => _connection;

        public IDbTransaction GetTransaction => _transaction;

        public IDbTransaction CreateTransaction()
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();

            _transaction = _connection.BeginTransaction();

            return _transaction;
        }
    }
}