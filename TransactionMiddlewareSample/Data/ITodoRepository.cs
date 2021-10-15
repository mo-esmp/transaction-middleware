using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TransactionMiddlewareSample.Models;

namespace TransactionMiddlewareSample.Data
{
    public interface ITodoListRepository
    {
        Task<int> AddTodoListAsync(TodoList todoList);

        Task<IEnumerable<TodoList>> GetTodoListAsync();
    }

    public class TodoListRepository : ITodoListRepository
    {
        private readonly SqlConnectionProvider _connectionProvider;
        private readonly IDbConnection _connection;

        public TodoListRepository(SqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _connection = connectionProvider.GetDbConnection;
        }

        public async Task<int> AddTodoListAsync(TodoList todoList)
        {
            const string command = "INSERT INTO TodoLists (Title) " +
                                   "OUTPUT Inserted.Id " +
                                   "VALUES (@Title)";

            var parameters = new DynamicParameters();
            parameters.Add("Title", todoList.Title, DbType.String);

            var id = await _connection.ExecuteScalarAsync<int>(command, parameters, _connectionProvider.GetTransaction);

            return id;
        }

        public Task<IEnumerable<TodoList>> GetTodoListAsync()
        {
            return _connection.ExecuteScalarAsync<IEnumerable<TodoList>>("SELECT * FROM TodoLists");
        }
    }
}