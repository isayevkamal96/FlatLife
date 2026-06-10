using System.Linq;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;
using FlatLife.Models.UserDTO;
using FlatLife.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlatLife.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RegisterResponseBody _registerResponseBody;
     
        private readonly ITokenService _tokenService;
        private readonly IUserRepositoryService _userRepository;

        public UserController(RegisterResponseBody registerResponseBody, ITokenService tokenService, IUserRepositoryService userRepository)
        {
            _registerResponseBody = registerResponseBody;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }


        [HttpPost("Register")]
        public ActionResult Register(RegisterRequestBody registerRequestBody)
        {

            var existingUsernameAndEmail = _userRepository.GetUsernameAndEmail(registerRequestBody.Username, registerRequestBody.Email);

            if (existingUsernameAndEmail != null)
            {

                if (existingUsernameAndEmail.Email != null)
                {
                    return Conflict("User with this Email already exists");
                }

                if (existingUsernameAndEmail.Username != null)
                {
                    return Conflict("Username already exists");
                }
            }

            User newUser = _registerResponseBody.Map(registerRequestBody);

            _userRepository.AddUser(newUser);

            var token = _tokenService.CreateToken(newUser);

            return StatusCode(StatusCodes.Status201Created, token);
        }

        [HttpPost("Login")]
        public ActionResult<string> Login(LoginRequestBody loginRequestBody)
        {
            if (loginRequestBody == null || string.IsNullOrEmpty(loginRequestBody.Email) || string.IsNullOrEmpty(loginRequestBody.Password))
            {
                return BadRequest("Please type the Email and Password");
            }

            var user = _userRepository.GetEmailAndPassword(loginRequestBody.Email, loginRequestBody.Password);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            string token = _tokenService.CreateToken(user);
            return Ok(token);
        }

        // Hilfsmethoden zur Validierung
        private bool IsValidName(string name)
        {
            // Überprüfen, ob der Name nur aus Buchstaben besteht
            return name.All(char.IsLetter);
        }

        private bool IsValidUsername(string username)
        {
            // Überprüfen, ob der Benutzername nur Buchstaben und Zahlen enthält
            return username.All(c => char.IsLetterOrDigit(c));
        }

        private bool IsValidPassword(string password)
        {
            // Passwort muss mindestens 8 Zeichen lang sein und Buchstaben und Zahlen enthalten
            return password.Length >= 8
                && password.Any(char.IsLetter)
                && password.Any(char.IsDigit);
        }
    }
}
