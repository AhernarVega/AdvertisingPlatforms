using AdvertisingPlatforms.Core.Domain.PersistenceContracts;

namespace AdvertisingPlatforms.Infrastructure.Storage;

public class DataStorage : IDataStorage
{
    /// <summary>
    /// Хранимые данные
    /// </summary>
    private Dictionary<string, HashSet<string>> _adPlatforms = [];

    /// <summary>
    /// Обновление текущего словаря локаций и платформ
    /// </summary>
    /// <param name="adPlatforms"> Новый словарь локаций и платформ </param>
    public void UpdateData(Dictionary<string, HashSet<string>> adPlatforms)
        => _adPlatforms = adPlatforms;

    /// <summary>
    /// Получение текущего словаря локаций и платформ
    /// </summary>
    /// <returns> Текущий словарь локаций и платформ </returns>
    public Dictionary<string, HashSet<string>> GetAdPlatforms()
        => _adPlatforms;
}