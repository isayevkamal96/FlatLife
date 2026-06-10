using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class PayloadReader
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public PayloadReader(JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }


    public int IDReader(string Header)
    {

        var tokenString = Header.Replace("Bearer ", "");

        var token = _jwtSecurityTokenHandler.ReadJwtToken(tokenString);
        var claims = token.Claims.ToList();

        var idClaim = claims.First(i => i.Type == ClaimTypes.NameIdentifier);

        return Convert.ToInt32(idClaim.Value);

    }
}
