using System.Collections.Generic;

namespace TransactionMiddlewareSample.Models
{
    public class TodoList
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IList<TodoItem> Items { get; set; } = new List<TodoItem>();
    }
}