namespace TransactionMiddlewareSample.Models
{
    public class TodoItem
    {
        public int Id { get; set; }

        public int TodoListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }
    }
}