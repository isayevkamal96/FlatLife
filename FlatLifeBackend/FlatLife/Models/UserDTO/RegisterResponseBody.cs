using System;
using FlatLife.Database.Entities;
using FlatLife.Mapping;

namespace FlatLife.Models.UserDTO;

public class RegisterResponseBody : BaseMapper<RegisterRequestBody, User>
{
    public override User Map(RegisterRequestBody registerReguestBody)
    {
        return new User
        {
            Username = registerReguestBody.Username,
            Password = registerReguestBody.Password,
            FirstName = registerReguestBody.FirstName,
            LastName = registerReguestBody.LastName,
            Email = registerReguestBody.Email,
            Birthday = registerReguestBody.Birthday
        };
    }
}
