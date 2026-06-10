using Xunit;
using Moq;
using FlatLife.Database.Entities;
using Microsoft.AspNetCore.Identity;
using FlatLife.Controllers;
using FlatLife.Models.UserDTO;
using FlatLife.Services;
using Microsoft.AspNetCore.Mvc;


namespace FlatLife.Tests;

public class LoginTest
{

    [Fact]
    public void Login_ReturnsCorrectToken()
    {
        //Arrange
        var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoia2FtYWxAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxIiwiZXhwIjoxNzQ0OTY1NDgyfQ.j_ch2z6qKbf_i7gFh3mRb8ZwP3fVroGEPh3YUA19zyk";
        var loginRequest = new LoginRequestBody
        {
            Email = "kamal@gmail.com",
            Password = "Isayevkamal1996#"
        };
        var testUser = new User {Email = loginRequest.Email, Password = loginRequest.Password};

        var mockTokenService = new Mock<ITokenService>();
        mockTokenService.Setup(t => t.CreateToken(It.IsAny<User>())).Returns(expectedToken);

        var mockUserRepository = new Mock<IUserRepositoryService>();
        mockUserRepository.Setup(r => r.GetEmailAndPassword(loginRequest.Email, loginRequest.Password)).Returns(testUser);

        var controller = new UserController(null!, mockTokenService.Object, mockUserRepository.Object);

        //Act
        var result = controller.Login(loginRequest);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result); 
        var actualToken = Assert.IsType<string>(okResult.Value);   
        Assert.Equal(expectedToken, actualToken);  
        // mockUserRepository.Verify(r => r.GetEmailAndPassword("kamil@gmail.com", "Isayevkamal1996#"), Times.Once);
    }

      [Fact]
    public void Login_ReturnsIncorrectToken()
    {
        //Arrange
        var loginRequest = new LoginRequestBody
        {
            Email = "kamal@gmail.com",
            Password = "Isayevkamal1996#"
        };

        var mockTokenService = new Mock<ITokenService>();

        var mockUserRepository = new Mock<IUserRepositoryService>();
        mockUserRepository.Setup(r => r.GetEmailAndPassword(loginRequest.Email, loginRequest.Password)).Returns((User?)null);

        var controller = new UserController(null!, mockTokenService.Object, mockUserRepository.Object);

        //Act
        var result = controller.Login(loginRequest);

        //Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("User not found", badRequestResult.Value);
    }
}