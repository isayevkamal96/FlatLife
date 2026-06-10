using System;
using FlatLife.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatLife.Database.ApplicationDbContext;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> User { get; set; }

    public DbSet<Flat> flat { get; set; }

    public DbSet<FlatUser> flatUser { get; set; }
    
    public DbSet<Bill> bill { get; set; }

    public DbSet<ToDoItem> ToDoItems { get; set; }

    public DbSet<FlatTask> FlatTask { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlatTask>()
            .HasOne(task => task.CurrentUser)
            .WithMany()
            .HasForeignKey(task => task.CurrentUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }

}
