using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TransactionMiddlewareSample.Models;

namespace TransactionMiddlewareSample.Data
{
    public interface ITodoItemRepository
    {
        Task<int> AddTodoItemAsync(TodoItem todoItem);

        Task<IEnumerable<TodoItem>> GetTodoItemsAsync();
    }

    public class TodoItemRepository : ITodoItemRepository
    {
        private readonly SqlConnectionProvider _connectionProvider;
        private readonly IDbConnection _connection;

        public TodoItemRepository(SqlConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _connection = connectionProvider.GetDbConnection;
        }

        public Task<int> AddTodoItemAsync(TodoItem todoItem)
        {
            const string command = "INSERT INTO TodoItems (Title, Note, TodoListId) VALUES (@Title, @Note, @TodoListId)";
            var parameters = new DynamicParameters();
            parameters.Add("Title", todoItem.Title, DbType.String);
            parameters.Add("Note", todoItem.Note, DbType.String);
            parameters.Add("TodoListId", todoItem.TodoListId, DbType.Int32);

            return _connection.ExecuteAsync(command, parameters, _connectionProvider.GetTransaction);
        }

        public Task<IEnumerable<TodoItem>> GetTodoItemsAsync()
        {
            return _connection.ExecuteScalarAsync<IEnumerable<TodoItem>>("SELECT * FROM TodoItems");
        }
    }
}