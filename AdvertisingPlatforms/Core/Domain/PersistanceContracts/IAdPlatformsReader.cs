namespace AdvertisingPlatforms.Infrastructure.Persistence;

public interface IAdPlatformsReader
{
    public Task<Dictionary<string, List<string>>> LoadAdPlatformsAsync(string pathToFile = "");
}