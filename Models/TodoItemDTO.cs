using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoApi.Models
{
    public class TodoItemDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
