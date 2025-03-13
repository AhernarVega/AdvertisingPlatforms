using AdvertisingPlatforms.Core.Domain.PersistenceContracts;
using AdvertisingPlatforms.Core.Services.Abstractions;

namespace AdvertisingPlatforms.Core.Services;

public class AdvertisingPlatformsService : IAdvertisingPlatformsService
{
    private readonly IAdPlatformsReader _adPlatformsReader;
    private readonly IDataStorage _dataStorage;
    
    public AdvertisingPlatformsService(IAdPlatformsReader adPlatformsReader, IDataStorage dataStorage)
    {
        _adPlatformsReader = adPlatformsReader;
        _dataStorage = dataStorage;
    }

    public async Task LoadingAdPlatformsFromFileAsync(string? pathToFile)
    {
        _dataStorage.UpdateData(await _adPlatformsReader.LoadAdPlatformsAsync(pathToFile));
    }

    public List<string> FindAdPlatformsByLocation(string location)
    {
        var data = _dataStorage.GetAdPlatforms();
        
        while (!string.IsNullOrEmpty(location))
        {
            if (data.TryGetValue(location, out var values))
            {
                return values.ToList();
            }

            location = location[..location.LastIndexOf('/')];
        }
        return [];
    }
}