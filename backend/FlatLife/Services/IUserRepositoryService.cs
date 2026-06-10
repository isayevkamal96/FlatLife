using System;
using FlatLife.Database.Entities;

namespace FlatLife.Services;

public interface IUserRepositoryService
{
    User? GetEmailAndPassword(string email, string password);
    User? GetUsernameAndEmail(string username, string email);
    User? AddUser(User user);
}
