namespace AdvertisingPlatforms.Core.Services.Abstractions;

public interface IAdvertisingPlatformsService
{
    public Task LoadingAdPlatformsFromFileAsync();
    public List<string> FindAdPlatformsByLocation(string location);
}