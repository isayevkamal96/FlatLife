using Xunit;
using Moq;
using FlatLife.Database.Entities;
using Microsoft.AspNetCore.Identity;
using FlatLife.Controllers;
using FlatLife.Models.UserDTO;
using FlatLife.Services;
using Microsoft.AspNetCore.Mvc;


namespace FlatLife.Tests;

public class RegisterTest
{

    [Fact]
    public void Register_ReturnsSucces()
    {
        //Arrange
        var registerRequest = new RegisterRequestBody
        {
            Username = "kamaliii",
            Password = "Isayevkamal1996",
            FirstName = "Kamal",
            LastName = "Isayev",
            Email = "kamalii@gmail.com",
            Birthday = DateOnly.FromDateTime(new DateTime(1996, 3, 15))
        };

        var registerResponseBody = new RegisterResponseBody();

        var testUser = new User { Username = registerRequest.Username, Email = registerRequest.Email };

        var mockToken = new Mock<ITokenService>();

        var mockUsername = new Mock<IUserRepositoryService>();
        mockUsername.Setup(u => u.GetUsernameAndEmail(registerRequest.Username, registerRequest.Email)).Returns((User?)null);

        var controller = new UserController(registerResponseBody, mockToken.Object, mockUsername.Object);

        //Act
        var result = controller.Register(registerRequest);

        //Assert
        Assert.NotNull(result);
        var objectResult = Assert.IsType<ObjectResult>(result);

        Assert.Equal(201, objectResult.StatusCode);

    }

    [Fact]
    public void Register_ReturnsFailure()
    {
        //Arrange
        var registerRequest = new RegisterRequestBody
        {
            Username = "kamaliii",
            Password = "Isayevkamal1996",
            FirstName = "Kamal",
            LastName = "Isayev",
            Email = "kamalii@gmail.com",
            Birthday = DateOnly.FromDateTime(new DateTime(1996, 3, 15))
        };

        var registerResponseBody = new RegisterResponseBody();

        var testUser = new User
        {
            Username = registerRequest.Username,
            Email = registerRequest.Email
        };

        var mockToken = new Mock<ITokenService>();

        var mockUsername = new Mock<IUserRepositoryService>();
        mockUsername.Setup(u => u.GetUsernameAndEmail(registerRequest.Username, registerRequest.Email)).Returns(testUser);

        var controller = new UserController(registerResponseBody, mockToken.Object, mockUsername.Object);

        //Act

        var result = controller.Register(registerRequest);

        //Assert
        Assert.NotNull(result);
        
        var objectResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal(409, objectResult.StatusCode);

    }
}