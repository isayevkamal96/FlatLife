using System;

namespace FlatLife.Models.UserDTO;

public class RegisterRequestBody
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string FirstName  { get; set; }
    public required string LastName  { get; set; }
    public required string Email { get; set; }
    public required DateOnly Birthday { get; set; }


}
