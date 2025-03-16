using AdvertisingPlatforms.Core.Domain.PersistenceContracts;
using AdvertisingPlatforms.Core.Services.Abstractions;

namespace AdvertisingPlatforms.Core.Services;


public class AdvertisingPlatformsService : IAdvertisingPlatformsService
{
    /// <summary>
    /// Объект для чтения данных из файла.
    /// </summary>
    private readonly IAdPlatformsReader _adPlatformsReader;
    /// <summary>
    /// Объект для хранения считанных данных.
    /// </summary>
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
        // Получение хранимого словаря с локациями и платформами
        var data = _dataStorage.GetAdPlatforms();
        
        // Ищем все подходящие платформы
        while (!string.IsNullOrEmpty(location))
        {
            // Попытка получить искомые платформы 
            if (data.TryGetValue(location, out var values))
            {
                return values.ToList();
            }

            // Если не получилось найти по искомой локации - поднимаемся на уровень вверх
            location = location[..location.LastIndexOf('/')];
        }
        // Если ничего не было найдено - вернуть пустой список 
        return [];
    }
}