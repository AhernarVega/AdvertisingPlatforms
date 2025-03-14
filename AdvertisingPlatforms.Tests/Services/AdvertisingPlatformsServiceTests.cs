using AdvertisingPlatforms.Core.Domain.PersistenceContracts;
using AdvertisingPlatforms.Core.Services;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace AdvertisingPlatformsTests.Services;

public class AdvertisingPlatformsServiceTests : TestBase
{
    private readonly Mock<IAdPlatformsReader> _adPlatformsReaderMock;
    private readonly Mock<IDataStorage> _dataStorageMock;
    private readonly AdvertisingPlatformsService _sut;

    public AdvertisingPlatformsServiceTests()
    {
        _adPlatformsReaderMock = Fixture.Freeze<Mock<IAdPlatformsReader>>();
        _dataStorageMock = Fixture.Freeze<Mock<IDataStorage>>();
        _sut = new AdvertisingPlatformsService(_adPlatformsReaderMock.Object, _dataStorageMock.Object);
    }

    [Fact]
    public async Task LoadingAdPlatformsFromFileAsync_WhenFileForReadExists_ShouldUpdateDataStorage()
    {
        // Arrange
        var expectedData = new Dictionary<string, HashSet<string>>
        {
            { "test/location", ["platform1", "platform2"] }
        };
        
        _adPlatformsReaderMock
            .Setup(x => x.LoadAdPlatformsAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedData);

        // Act
        await _sut.LoadingAdPlatformsFromFileAsync(It.IsAny<string>());

        // Assert
        _dataStorageMock.Verify(x => x.UpdateData(expectedData), Times.Once);
    }
    
    [Fact]
    public async Task LoadingAdPlatformsFromFileAsync_WhenFileForReadNotExists_ShouldUpdateDataStorage()
    {
        // Arrange
        const string pathToFile = "test.json";
        var expectedData = new Dictionary<string, HashSet<string>>
        {
            { "test/location", ["platform1", "platform2"] }
        };
        
        _adPlatformsReaderMock
            .Setup(x => x.LoadAdPlatformsAsync(It.IsAny<string>()))
            .ThrowsAsync(new FileNotFoundException());

        // Act
        var result = async () => await _sut.LoadingAdPlatformsFromFileAsync(pathToFile);

        // Assert
        await result.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public void FindAdPlatformsByLocation_WhenLocationExists_ShouldReturnPlatforms()
    {
        // Arrange
        const string location = "test/location";
        var expectedPlatforms = new HashSet<string> { "platform1", "platform2" };
        var data = new Dictionary<string, HashSet<string>>
        {
            { location, expectedPlatforms }
        };

        _dataStorageMock
            .Setup(x => x.GetAdPlatforms())
            .Returns(data);

        // Act
        var result = _sut.FindAdPlatformsByLocation(location);

        // Assert
        result.Should().BeEquivalentTo(expectedPlatforms);
    }

    [Fact]
    public void FindAdPlatformsByLocation_WhenLocationNotExists_ShouldReturnEmptyList()
    {
        // Arrange
        const string location = "/nonexistent/location";
        var data = new Dictionary<string, HashSet<string>>();

        _dataStorageMock
            .Setup(x => x.GetAdPlatforms())
            .Returns(data);

        // Act
        var result = _sut.FindAdPlatformsByLocation(location);

        // Assert
        result.Should().BeEmpty();
    }
} 