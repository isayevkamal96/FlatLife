using FlatLife.Database.ApplicationDbContext;
using Microsoft.IdentityModel.Tokens;

public class RandomShortNameGenerator
{
    private ApplicationDbContext _dbContext;

    public RandomShortNameGenerator(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public string RandomShortNameGen()
    {
        List<string> character = new List<string>(new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                                                                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                                                                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                                                "!", "@", "#", "$", "%", "^", "&"});

        Random random = new Random();
        string flatShortName = "";

        for (int i = 0; i < 6; i++)
        {
            int randomNumber = random.Next(0, 68);

            flatShortName += character[randomNumber];
        }

        var databaseOutput = _dbContext.flat.FirstOrDefault(p => p.flatShortName == flatShortName);

        if (databaseOutput != null)
        {
            return RandomShortNameGen();
        }
        else
        {
            return flatShortName;
        }
    }
}