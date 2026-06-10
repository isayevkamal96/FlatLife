using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using FlatLife.Models.FlatDTO;
using FlatLife.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlatLife.Controllers
{
    [Route("api/FlatTask")]
    [ApiController]
    [Authorize]
    public class FlatTaskController : ControllerBase
    {
        private readonly FlatTaskResponseBodyMapper _flatTaskResponseBodyMapper;
        private readonly FlatTaskService _flatTaskService;
        private readonly ApplicationDbContext _db;
        private readonly PayloadReader _payloadReader;

        public FlatTaskController(FlatTaskResponseBodyMapper flatTaskResponseBodyMapper, ApplicationDbContext db, FlatTaskService flatTaskService, PayloadReader payloadReader)
        {
            _flatTaskResponseBodyMapper = flatTaskResponseBodyMapper;
            _flatTaskService = flatTaskService;
            _payloadReader = payloadReader;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<FlatTaskResponseBody>>> GetTasks()
        {
            try
            {
                var userId = GetUserIdFromPayload();

                if (userId == 0)
                {
                    return Unauthorized("Invalid or missing user token");
                }

                var activeFlatId = await _db.flatUser
                    .Where(fu => fu.userid == userId && fu.IsActive)
                    .Select(fu => fu.flatID)
                    .FirstOrDefaultAsync();

                if (activeFlatId == 0)
                {
                    return NotFound("The user is not applied to any Flat");
                }

                var tasks = await _db.FlatTask
                    .Include(t => t.CurrentUser)
                    .Where(t => t.FlatId == activeFlatId)
                    .ToListAsync();

                _flatTaskService.TaskRotation();

                await _db.SaveChangesAsync();
                var response = tasks.Select(task => _flatTaskResponseBodyMapper.Map(task)).ToList();
                return Ok(response);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured while accessing the database");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occured");

            }
        }


        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<FlatTaskResponseBody>> PostTask([FromBody] FlatTaskRequestBody flatTaskRequestBody)
        {
            try
            {
                if (flatTaskRequestBody == null)
                {
                    return BadRequest();
                }

                var userId = GetUserIdFromPayload();
                if (userId == 0)
                {
                    return Unauthorized("Invalid or missing user token");
                }
                var activeFlatId = await _db.flatUser
                    .Where(fu => fu.userid == userId && fu.IsActive)
                    .Select(fu => fu.flatID)
                    .FirstOrDefaultAsync();

                if (activeFlatId == 0)
                {
                    return BadRequest("User is not belong to any Flat");
                }

                var createdTask = await _flatTaskService.CreateTask(flatTaskRequestBody, activeFlatId);
                if (createdTask == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create the task");
                }

                return StatusCode(StatusCodes.Status201Created, createdTask);

            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, dbEx.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }

        }

        [HttpDelete("{id}", Name = "DeleteTask")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<FlatTaskResponseBody>> DeleteTask(int id)
        {
            await _flatTaskService.DeleteTask(id);

            return Ok();
        }

        [HttpPut("{id}", Name = "UpdateTask")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<FlatTaskResponseBody>> UpdateTask(int id, [FromBody] FlatTaskPutBody flatTaskPutBody)
        {
            try
            {
                var task = await _db.FlatTask.FindAsync(id);

                if (task == null)
                {
                    return NotFound();
                }

                var userId = GetUserIdFromPayload();
                if (userId == 0)
                {
                    return Unauthorized("Invalid or missing user token");
                }
                var activeFlatId = await _db.flatUser
                    .Where(fu => fu.userid == userId && fu.IsActive)
                    .Select(fu => fu.flatID)
                    .FirstOrDefaultAsync();

                if (activeFlatId == 0)
                {
                    return BadRequest("User is not belong to any Flat");
                }

                if (task.FlatId != activeFlatId)
                {
                    throw new ArgumentException("You do not belong to this Flat");
                }

                var updatedTask = await _flatTaskService.UpdateTask(id, flatTaskPutBody, activeFlatId);

                return Ok(updatedTask);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured while accessing the database");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occured");

            }

        }

        private int GetUserIdFromPayload()
        {
            var token = Request.Headers["Authorization"].ToString();
            return _payloadReader.IDReader(token);
        }
    }
}
