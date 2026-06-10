using System.IdentityModel.Tokens.Jwt;
using FlatLife.Controllers;
using FlatLife.Models.UserDTO;

namespace FlatLife.Tests;

public class IDReaderTest
{
    [Fact]
    public void PayloadReader_IDReader_ReturnsCorrectTokenID()
    {
        //Arrange
        var handler = new JwtSecurityTokenHandler();
        var reader = new PayloadReader(handler);

        //Act
        var result = reader.IDReader("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoia2FtYWxAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxIiwiZXhwIjoxNzQ0OTY1NDgyfQ.j_ch2z6qKbf_i7gFh3mRb8ZwP3fVroGEPh3YUA19zyk");

        //Assert
        Assert.Equal(1, result);

    }

    [Theory]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoia2FtYWxAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxIiwiZXhwIjoxNzQ0OTY1NDgyfQ.j_ch2z6qKbf_i7gFh3mRb8ZwP3fVroGEPh3YUA19zyk", 1)]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoibWF4QGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMTIiLCJleHAiOjE3NDQ5ODU1NjN9.6YSaHmIvqya9n2XL4ET_R67bc8HwoVCNI_Saz7bw4zY", 12)]
    public void PayloadReader_IDReader_ReturnsMultipleCorrectTokenIDs(string token, int expected)
    {
        //Arrange 
        var handler = new JwtSecurityTokenHandler();
        var reader = new PayloadReader(handler);

        //Act
        var result = reader.IDReader(token);

        //Assert
        Assert.Equal(expected, result);
    }
}

    

  