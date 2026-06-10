using System;
using FlatLife.Database.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;

namespace FlatLife.Test;

public class Test
{
    public static void TestMethode()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql().Options;
        var testDb = new ApplicationDbContext(options);
    }

}
