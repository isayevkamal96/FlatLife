namespace FlatLife.Tests;

public class DummyTest
{
    [Fact]
    public void IDReader_Dummy()
    {
        PayloadReader reader = new PayloadReader(new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler());
        Assert.ThrowsAny<ArgumentException>(() => reader.IDReader(""));
    }
}
