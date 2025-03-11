namespace AdvertisingPlatforms.Core.Services;

public interface IAdvertisingPlatformsService
{
    public Task LoadingAdPlatformsFromFileAsync();
    public List<string> FindAdPlatformsByLocation(string location);
}