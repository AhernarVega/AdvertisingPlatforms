namespace AdvertisingPlatforms.Core.Domain.PersistenceContracts;

public interface IAdPlatformsReader
{
    public Task<Dictionary<string, HashSet<string>>> LoadAdPlatformsAsync(string pathToFile = "");
}