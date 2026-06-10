using System;
using FlatLife.Database.Entities;

namespace FlatLife.Services;

public interface ITokenService
{
    string CreateToken(User user);
}
