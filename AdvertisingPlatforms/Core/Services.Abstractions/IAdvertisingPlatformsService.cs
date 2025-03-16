namespace AdvertisingPlatforms.Core.Services.Abstractions;

/// <summary>
/// Контракт для сервиса, содержащего логику работы приложения.
/// </summary>
public interface IAdvertisingPlatformsService
{
    /// <summary>
    /// Обновление данных:
    /// Чтение их из файла и полная замена существующих данных считанными.
    /// </summary>
    /// <param name="pathToFile"> Путь к файлу </param>
    public Task LoadingAdPlatformsFromFileAsync(string? pathToFile);
    
    /// <summary>
    /// Поиск платформ, работающих в указанной локации.
    /// </summary>
    /// <param name="location"> Локация для поиска платформ </param>
    /// <returns> Список искомых платформ </returns>
    public List<string> FindAdPlatformsByLocation(string location);
}