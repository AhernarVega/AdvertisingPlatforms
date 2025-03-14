using AdvertisingPlatforms.Core.Services.Abstractions;
using AdvertisingPlatforms.Presentation.API.Controllers;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdvertisingPlatformsTests.Controllers;

public class AdvertisingPlatformsControllerTests : TestBase
{
    private readonly Mock<IAdvertisingPlatformsService> _serviceMock;
    private readonly Mock<IValidator<string>> _validatorMock;
    private readonly AdvertisingPlatformsController _sut;

    public AdvertisingPlatformsControllerTests()
    {
        _serviceMock = Fixture.Freeze<Mock<IAdvertisingPlatformsService>>();
        _validatorMock = Fixture.Freeze<Mock<IValidator<string>>>();
        _sut = new AdvertisingPlatformsController(_serviceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task LoadingAdPlatformsFromFileAsync_WhenFileForReadExists_ShouldReturnOk()
    {
        // Arrange
        const string pathToFile = "test.json";

        _serviceMock
            .Setup(x => x.LoadingAdPlatformsFromFileAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _sut.LoadingAdPlatformsFromFileAsync(pathToFile);

        // Assert
        result.Should().BeOfType<OkResult>();
        _serviceMock.Verify(x => x.LoadingAdPlatformsFromFileAsync(pathToFile), Times.Once);
    }
    
    [Fact]
    public async Task LoadingAdPlatformsFromFileAsync_WhenFileForReadNotExists_ShouldReturnOk()
    {
        // Arrange
        const string pathToFile = "test.json";

        _serviceMock
            .Setup(x => x.LoadingAdPlatformsFromFileAsync(It.IsAny<string>()))
            .ThrowsAsync(new FileNotFoundException());

        // Act
        var action = async Task () => await _sut.LoadingAdPlatformsFromFileAsync(pathToFile);
        // var result = await _sut.LoadingAdPlatformsFromFileAsync(pathToFile);

        // Assert
        await action.Should().ThrowAsync<FileNotFoundException>();
        _serviceMock.Verify(x => x.LoadingAdPlatformsFromFileAsync(pathToFile), Times.Once);
    }

    [Fact]
    public void FindAdPlatformsByLocationAsync_WhenValidationPasses_ShouldReturnOkWithPlatforms()
    {
        // Arrange
        const string location = "test/location";
        var expectedPlatforms = new List<string> { "platform1", "platform2" };
        
        _validatorMock
            .Setup(x => x.Validate(location))
            .Returns(new ValidationResult());

        _serviceMock
            .Setup(x => x.FindAdPlatformsByLocation(location))
            .Returns(expectedPlatforms);

        // Act
        var result = _sut.FindAdPlatformsByLocationAsync(location);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedPlatforms);
    }

    [Fact]
    public void FindAdPlatformsByLocationAsync_WhenValidationFails_ShouldReturnBadRequest()
    {
        // Arrange
        const string location = "invalid/location";
        const string validationError = "Invalid location format";
        var validationResult = new ValidationResult(
            [new ValidationFailure("location", validationError)]
        );

        _validatorMock
            .Setup(x => x.Validate(location))
            .Returns(validationResult);

        // Act
        var result = _sut.FindAdPlatformsByLocationAsync(location);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be($"{validationError}\n");
    }
} 