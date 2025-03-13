namespace AdvertisingPlatforms.Core.Services.Abstractions;

public interface IAdvertisingPlatformsService
{
    public Task LoadingAdPlatformsFromFileAsync(string? pathToFile);
    public List<string> FindAdPlatformsByLocation(string location);
}