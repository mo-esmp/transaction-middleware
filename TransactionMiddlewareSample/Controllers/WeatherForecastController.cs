using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionMiddlewareSample.Data;
using TransactionMiddlewareSample.Middlewares;
using TransactionMiddlewareSample.Models;

namespace TransactionMiddlewareSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("todo-list")]
        public async Task<IEnumerable<object>> Get([FromServices] ITodoListRepository repository)
        {
            return await repository.GetTodoListAsync();
        }

        [Transaction]
        [HttpPost("todo-list")]
        public async Task<IActionResult> Post(
            [FromBody] TodoList todoList,
            [FromServices] ITodoListRepository todoListRepository,
            [FromServices] ITodoItemRepository todoItemRepository

            )
        {
            var id = await todoListRepository.AddTodoListAsync(todoList);

            if (todoList.Items == null || !todoList.Items.Any())
                return Ok();

            foreach (var item in todoList.Items)
            {
                item.TodoListId = id;
                await todoItemRepository.AddTodoItemAsync(item);
            }

            return Ok();
        }
    }
}