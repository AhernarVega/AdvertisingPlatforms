namespace AdvertisingPlatforms.Core.Domain.PersistenceContracts;

/// <summary>
/// Контракт для работы с хранением данных
/// </summary>
public interface IDataStorage
{
    /// <summary>
    /// Обновляет текущий список платформ, расположенных по определенным адресам из файла.
    /// </summary>
    /// <param name="adPlatforms"> Новые данные, где ключи - локации, значение - hasSet платформ </param>
    public void UpdateData(Dictionary<string, HashSet<string>> adPlatforms);
    
    /// <summary>
    /// Метод для получения текущего списка платформ, расположенных по определенным адресам
    /// </summary>
    /// <returns> Список платформ, расположенных по определенным адресам </returns>
    public Dictionary<string, HashSet<string>> GetAdPlatforms();
}