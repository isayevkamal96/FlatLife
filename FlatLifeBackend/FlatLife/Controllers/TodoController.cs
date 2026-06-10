using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using FlatLife.Models.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlatLife.Controllers
{
    [Route("api/todo")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PayloadReader _payloadReader;

        public TodoController(ApplicationDbContext context, PayloadReader payloadReader)
        {
            _context = context;
            _payloadReader = payloadReader;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                int userId = GetUserIdFromPayload();
                if (userId == 0)
                {
                    return Unauthorized("Invalid token or user not authenticated.");
                }

                var activeFlatIdQuery =
                    from fu in _context.flatUser
                    where fu.userid == userId && fu.IsActive
                    select fu.flatID;
                int activeFlatId = await activeFlatIdQuery.FirstOrDefaultAsync();

                if (activeFlatId == 0)
                {
                    return NotFound("User is not assigned to an active flat.");
                }

                var flattasksQuery =
                    from t in _context.ToDoItems
                    where t.FlatID == activeFlatId
                    select t;
                List<ToDoItem> flattasks = await flattasksQuery.ToListAsync();

                var taskwithUsername = new List<ToDoItemWithUserDto>();

                foreach (var task in flattasks)
                {
                    Console.WriteLine($"Task: {task.Task}, ID: {task.Id}");

                    var userNameQuery =
                        from u in _context.User
                        where u.Id == task.UserId
                        select u.Username;
                    string userName = await userNameQuery.FirstOrDefaultAsync() ?? "Unbekannt";

                    taskwithUsername.Add(
                        new ToDoItemWithUserDto
                        {
                            Id = task.Id,
                            Task = task.Task,
                            IsChecked = task.IsChecked,
                            CreatedDate = task.CreatedDate,
                            UpdatedDate = task.UpdatedDate,
                            FlatID = task.FlatID,
                            CreatedByUserName = userName
                        }
                    );
                }

                return Ok(taskwithUsername);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateToDoItem([FromBody] CreateToDoItemDto newItemDto)
        {
            int userId = _payloadReader.IDReader(Request.Headers["Authorization"]);
            if (userId == 0)
            {
                return Unauthorized("User not authenticated.");
            }

            var activeFlatIdQuery =
                from fu in _context.flatUser
                where fu.userid == userId && fu.IsActive
                select fu.flatID;
            int activeFlatId = await activeFlatIdQuery.FirstOrDefaultAsync();

            if (activeFlatId == 0)
            {
                return NotFound("User is not associated with any active flat.");
            }

            var userNameQuery = from u in _context.User where u.Id == userId select u.Username;
            string userName = await userNameQuery.FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("UserName is not available.");
            }

            var toDoItem = new ToDoItem
            {
                Task = newItemDto.Task,
                IsChecked = newItemDto.IsChecked,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                UserId = userId,
                FlatID = activeFlatId,
                CreatedByUserName = userName
            };

            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();

            return Ok(toDoItem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            int userId = _payloadReader.IDReader(Request.Headers["Authorization"]);
            if (userId == 0)
            {
                return Unauthorized("Invalid token or user not authenticated.");
            }

            ToDoItem task = await _context.ToDoItems.FindAsync(id);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            var activeFlatIdQuery =
                from fu in _context.flatUser
                where fu.userid == userId && fu.IsActive
                select fu.flatID;
            int activeFlatId = await activeFlatIdQuery.FirstOrDefaultAsync();

            if (activeFlatId == 0)
            {
                return NotFound("User is not assigned to an active flat.");
            }

            if (task.FlatID != activeFlatId)
            {
                return Unauthorized("You are not authorized to delete this task.");
            }

            _context.ToDoItems.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Edit a task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int id,
            [FromBody] UpdateToDoItemDto updatedTaskDto
        )
        {
            if (id == 0)
            {
                return BadRequest("Invalid task ID.");
            }

            int userId = GetUserIdFromPayload();
            if (userId == 0)
            {
                return Unauthorized("Invalid token or user not authenticated.");
            }

            ToDoItem task = await _context.ToDoItems.FindAsync(id);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            var activeFlatIdQuery =
                from fu in _context.flatUser
                where fu.userid == userId && fu.IsActive
                select fu.flatID;
            int activeFlatId = await activeFlatIdQuery.FirstOrDefaultAsync();

            if (activeFlatId == 0 || task.FlatID != activeFlatId)
            {
                return Unauthorized("You are not authorized to update this task.");
            }
            if (task == null || string.IsNullOrEmpty(updatedTaskDto.Task))
            {
                return BadRequest("Task cannot be empty");
            }

            task.Task = updatedTaskDto.Task;
            task.IsChecked = updatedTaskDto.IsChecked;
            task.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        private int GetUserIdFromPayload()
        {
            string token = Request.Headers["Authorization"].ToString();
            return _payloadReader.IDReader(token);
        }
    }
}
