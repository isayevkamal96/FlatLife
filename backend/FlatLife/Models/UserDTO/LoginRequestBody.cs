using System;

namespace FlatLife.Models.UserDTO;

public class LoginRequestBody
{
    public string Email { get; set; }
    public string Password { get; set; }

}
