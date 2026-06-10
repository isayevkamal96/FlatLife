using System;
using FlatLife.Database.ApplicationDbContext;
using FlatLife.Database.Entities;

namespace FlatLife.Services;

public class UserRepositoryService : IUserRepositoryService
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepositoryService(ApplicationDbContext dbContext){
        _dbContext = dbContext;
    }

    public User? GetEmailAndPassword(string email, string password)
    {
        return _dbContext.User.FirstOrDefault(u => u.Email == email && u.Password == password);
    }

    public User? GetUsernameAndEmail(string username, string email)
    {
        return _dbContext.User.FirstOrDefault(u => u.Username == username && u.Email == email);
    }
    public User? AddUser(User user)
    {
        var entityEntry = _dbContext.Add(user);
        _dbContext.SaveChanges();
        return entityEntry.Entity;
    }

}
