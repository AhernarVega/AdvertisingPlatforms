using AdvertisingPlatforms.Infrastructure.Persistence;

namespace AdvertisingPlatforms.Core.Services;

public class AdvertisingPlatformsPlatformsService
{
    private readonly IAdPlatformsReader _adPlatformsReader;
    // Допустил, что могу хранить in-memory коллекцию в сервисах,
    // поскольку логика фильтрации происходит в сервисе,
    // то выделение класса просто под хранение одной коллекции в данному случе излишне
    private Dictionary<string, HashSet<string>> _adPlatforms;
    
    public AdvertisingPlatformsPlatformsService(IAdPlatformsReader adPlatformsReader)
    {
        _adPlatformsReader = adPlatformsReader;
        _adPlatforms = [];
    }

    public async Task LoadingAdPlatformsFromFileAsync()
    {
        _adPlatforms = await _adPlatformsReader.LoadAdPlatformsAsync();
    }

    public async Task<List<string>> FindAdPlatformsByLocationAsync(string location)
    {
        while (!string.IsNullOrEmpty(location))
        {
            if (_adPlatforms.TryGetValue(location, out var values))
            {
                return values.ToList();
            }

            location = location[..location.LastIndexOf('/')];
        }
        return [];
    }
}