namespace AdvertisingPlatforms.Core.Domain.PersistenceContracts;

/// <summary>
/// Контракт с методом для чтения списка платформ и их локаций из файла
/// </summary>
public interface IAdPlatformsReader
{
    /// <summary>
    /// Метод, считывающий платформы и их локации.
    /// Медленный, поскольку предполагается редкий вызов.
    /// Учитывает требуемый формат ввод данных: несоответствующие строки пропускаются и логируются.
    /// </summary>
    /// <param name="pathToFile"> Путь к файлу с платформами и локациями </param>
    /// <returns>
    /// Словарь, где ключами являются локации, а значениями список платформ,
    /// работающих на этих локациях
    /// </returns>
    public Task<Dictionary<string, HashSet<string>>> LoadAdPlatformsAsync(string? pathToFile = "");
}