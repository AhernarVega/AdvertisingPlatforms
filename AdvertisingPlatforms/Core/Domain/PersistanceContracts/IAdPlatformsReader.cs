namespace AdvertisingPlatforms.Infrastructure.Persistence;

public interface IAdPlatformsReader
{
    public Task<Dictionary<string, HashSet<string>>> LoadAdPlatformsAsync(string pathToFile = "");
}