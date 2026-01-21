using CrochetAI.Api.Controllers;
using CrochetAI.Api.DTOs;
using CrochetAI.Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CrochetAI.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IValidator<RegisterRequest>> _mockRegisterValidator;
    private readonly Mock<IValidator<LoginRequest>> _mockLoginValidator;
    private readonly Mock<IValidator<RefreshTokenRequest>> _mockRefreshTokenValidator;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockRegisterValidator = new Mock<IValidator<RegisterRequest>>();
        _mockLoginValidator = new Mock<IValidator<LoginRequest>>();
        _mockRefreshTokenValidator = new Mock<IValidator<RefreshTokenRequest>>();

        _controller = new AuthController(
            _mockAuthService.Object,
            _mockRegisterValidator.Object,
            _mockLoginValidator.Object,
            _mockRefreshTokenValidator.Object);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Test123!"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockRegisterValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        var response = new AuthResponse
        {
            User = new UserDto { Id = "user-123", Username = "testuser", Email = "test@example.com" },
            AccessToken = "token",
            RefreshToken = "refresh"
        };

        _mockAuthService.Setup(s => s.RegisterAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task Register_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterRequest();
        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));

        _mockRegisterValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockLoginValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        var response = new AuthResponse
        {
            User = new UserDto { Id = "user-123" },
            AccessToken = "token",
            RefreshToken = "refresh"
        };

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@example.com", Password = "wrong" };
        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockLoginValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(validationResult);

        _mockAuthService.Setup(s => s.LoginAsync(request))
            .ThrowsAsync(new UnauthorizedAccessException());

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
