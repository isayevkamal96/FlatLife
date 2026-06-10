using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using FlatLife.Mapping;
using FlatLife.Models.UserDTO;
using FlatLife.Models.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FlatLife.Controllers
{
    [Authorize]
    [Route("api/flat")]
    [ApiController]
    public class FlatManagmentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RandomShortNameGenerator _randomShortNameGenerator;
        private readonly PayloadReader _payloadReader;

        public FlatManagmentController(
            ApplicationDbContext dbContext,
            RandomShortNameGenerator randomShortNameGenerator,
            PayloadReader payloadReader
        )
        {
            _dbContext = dbContext;
            _randomShortNameGenerator = randomShortNameGenerator;
            _payloadReader = payloadReader;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFlat(CreateFlatBody createFlatBody)
        {
            if (createFlatBody == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "No input is provided");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }
            if (createFlatBody.flatName.Length < 3)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "Flat name is not long enough pls use Minimum 4 character"
                );
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            var id = _payloadReader.IDReader(authHeader);
            var user = _dbContext.User.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Not Found");
            }

            var existingUserFlats = _dbContext.flatUser.Where(fu => fu.userid == user.Id).ToList();
            foreach (var existingFlatUser in existingUserFlats)
            {
                existingFlatUser.IsActive = false;
            }

            string shortname = _randomShortNameGenerator.RandomShortNameGen();
            Flat flat = new Flat { flatName = createFlatBody.flatName, flatShortName = shortname, };
            _dbContext.flat.Add(flat);
            await _dbContext.SaveChangesAsync();

            var CreatedFlat = _dbContext.flat.FirstOrDefault(p => p.flatShortName == shortname);
            if (CreatedFlat == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Not Found");
            }
            FlatUser flatUser = new FlatUser
            {
                userid = user.Id,
                flatID = CreatedFlat.id,
                IsActive = true
            };

            _dbContext.flatUser.Add(flatUser);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, flatUser);
        }

        [HttpPut("join")]
        public async Task<IActionResult> JoinFlat(JoinFlatBody joinFlatBody)
        {
            if (joinFlatBody == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "No input is provided");
            }
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }
            var authHeader = Request.Headers["Authorization"].ToString();
            var id = _payloadReader.IDReader(authHeader);
            var user1 = _dbContext.User.FirstOrDefault(x => x.Id == id);
            var flat1 = _dbContext.flat.FirstOrDefault(x =>
                x.flatShortName == joinFlatBody.flatShortName
            );

            if (user1 == null || flat1 == null)
            {
                return NotFound("User or Flat not found");
            }
            
            var existingUserFlats = _dbContext.flatUser.Where(fu => fu.userid == user1.Id).ToList();
            foreach (var existingFlatUser in existingUserFlats)
            {
                existingFlatUser.IsActive = false;
            }
            FlatUser flatUser = new FlatUser
            {
                userid = user1.Id,
                flatID = flat1.id,
                IsActive = true
            };
            _dbContext.flatUser.Add(flatUser);
            await _dbContext.SaveChangesAsync();

            return StatusCode(StatusCodes.Status200OK, flatUser);
        }
    }
}
