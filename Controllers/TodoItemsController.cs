using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ToDoApi.Models;

namespace ToDoApi.Controllers
{
    [Route("list")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            return await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }


        [Route("/todos/{id}/toggle_status")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItemCompleted(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.IsComplete = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }
            return Ok();
        }


        [Route("/todos/toggle_all")]
        [HttpPut]
        public async Task<IActionResult> UpdateAllItemsCompleted()
        {
            var items = _context.TodoItems;
            foreach (var item in items)
            {
                item.IsComplete = true;

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
            }
            return Ok();
        }

        [HttpPut("{id}")]
        [Route("/todos/{id}")]
        public async Task<IActionResult> UpdateTitle(long id, [FromForm] IFormCollection value)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Name = value["todo-title"];

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }
            return Ok();
        }



        [Route("/addTodo")]
        [HttpPost]
        [ActionName("addTodo")]
        [Consumes("application/x-www-form-urlencoded")]

        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem([FromForm] IFormCollection value)
        {

            var todoItem = new TodoItem
            {
                IsComplete = false,
                Name =value["todo-title"]
            };

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        //[Route("/addTodo")]
        //[HttpPost]
        //[ActionName("addTodo")]
        //[Consumes("application/x-www-form-urlencoded")]
        //public async Task<ActionResult<TodoItemDTO>> CreateTodoItem([FromBody]string data)
        //{
        //    var name = data.Split("todo-title")[1];
        //    var todoItem = new TodoItem
        //    {
        //        IsComplete = false,
        //        Name = name
        //    };

        //    _context.TodoItems.Add(todoItem);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(
        //        nameof(GetTodoItem),
        //        new { id = todoItem.Id },
        //        ItemToDTO(todoItem));
        //}

        [HttpPost]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> Update()
        {
            return await _context.TodoItems
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        [HttpDelete("{num}")]
        [Route("/todo/{num}")]
        public async Task<IActionResult> DeleteTodoItem(long num)
        {
            Console.WriteLine(num);
            var todoItem = await _context.TodoItems.FindAsync(num);

            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("/todos/completed")]
        public async Task<IActionResult> DeleteAll()
        {
            var items = _context.TodoItems;
            foreach (var item in items)
            {
                items.Remove(item);

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
            }
            return Ok();
        }

        private bool TodoItemExists(long id) =>
             _context.TodoItems.Any(e => e.Id == id);

        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Title = todoItem.Name,
                Completed = todoItem.IsComplete
            };
    }
}

