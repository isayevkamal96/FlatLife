using System;
using FlatLife.Database.Entities;
using FlatLife.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace FlatLife.Models.UserDTO;

public class LoginResponseBody : BaseMapper<LoginRequestBody, User>
{
    public override User Map(LoginRequestBody loginRequestBody)
    {
        return new User
        {
            Email = loginRequestBody.Email,
            Password = loginRequestBody.Password
        };
    }
}