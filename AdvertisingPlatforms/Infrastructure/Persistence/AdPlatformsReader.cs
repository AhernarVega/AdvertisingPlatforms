using System.Text.RegularExpressions;
using AdvertisingPlatforms.Core.Domain.PersistenceContracts;

namespace AdvertisingPlatforms.Infrastructure.Persistence;

public partial class AdPlatformsReader : IAdPlatformsReader
{
    private readonly ILogger<string> _logger;

    public AdPlatformsReader(ILogger<string> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Промежуточный метод для вынесенной логики получения всех "родительских локаций".
    /// </summary>
    /// <param name="sourcePlatforms"> Изначальная локация </param>
    /// <returns> Список возможных комбинаций локаций </returns>
    private IEnumerable<string> GetParentLocations(string sourcePlatforms)
    {
        // Разбиение изначальной локации по разделителю '/'
        var parts = sourcePlatforms.Split('/');
        // Формирование списка возможных локаций
        return Enumerable
            .Range(1, parts.Length - 1)
            .Select(i => string.Join("/", parts.Take(i)));
    }

    /// <summary>
    /// Метод, дополняющий существующий словарь списком "родительских локаций" уже существующих.
    /// </summary>
    /// <param name="sourceData"> Изначальный словарь </param>
    /// <returns> Новый словарь после создания дополнения </returns>
    private Dictionary<string, HashSet<string>> ProcessDictionary(Dictionary<string, HashSet<string>> sourceData)
    {
        // Подготовка нового словаря
        var result = new Dictionary<string, HashSet<string>>();

        // Перебор по ключам текущего словаря
        foreach (var key in sourceData.Keys)
        {
            // Получение текущего HashSet платформ
            var values = new HashSet<string>(sourceData[key]);

            // Перебор всех возможных вариантов локаций
            foreach (var parentKey in GetParentLocations(key))
            {
                // Если такая локация уже существует, добавляем к ней новые данные - дополняем HashSt платформ
                if (sourceData.TryGetValue(parentKey, out var parentValues))
                {
                    values.UnionWith(parentValues);
                }
            }
            
            // Иначе создаем новую строку словаря с ключем - текущей платформой
            result[key] = values;
        }

        return result;
    }
    
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
    /// <exception cref="FileNotFoundException"> Исключение, возникающие при указании неверного пути к файлу </exception>
    public async Task<Dictionary<string, HashSet<string>>> LoadAdPlatformsAsync(string? pathToFile)
    {
        // Формирование пути. Если путь не указан - использовать стандартное
        pathToFile = string.IsNullOrEmpty(pathToFile)
            ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\AdPlatforms.txt")
            : pathToFile;

        // Если файл по указанному пути не найден - сгенерировать исключение FileNotFoundException
        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException($"{Path.GetFileName(pathToFile)} file not found");
        }

        // Подготовка словаря с платформами и их локациями
        var adPlatforms = new Dictionary<string, HashSet<string>>();

        // Создание потока для чтения файла
        using var reader = new StreamReader(pathToFile);

        // Списки, содержащие платформы и локации для проверки на повторное использование
        var checkPlatforms = new List<string>();
        var checkLocations = new List<string>();
        // Текущая проверяемая строка
        var lines = 0;

        // Читаем файл построчно, пока это возможно
        while (await reader.ReadLineAsync() is { } line)
        {
            // Увеличиваем счетчик строки
            ++lines;

            // Проверяем текущую строку на наличие полезной информации
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // При помощи регулярного выражения проверяем формат данных
            var regex = CheckPlatformLocationRegex();
            if (!regex.Match(line).Success)
            {
                _logger.LogWarning($"{lines}: The string had an incorrect format. Expected format:" +
                                   $" [PlatformName]: [/location1], [/location2], .. [/locationN]");
            }

            // Логирование строки, если пропущено :
            var colonIndex = line.IndexOf(':');
            if (colonIndex == -1)
            {
                _logger.LogWarning("The \":\" is missing");
                continue;
            }

            // Логирование строки, если не указана платформа
            var adPlatform = line[..colonIndex];
            if (string.IsNullOrEmpty(adPlatform))
            {
                _logger.LogWarning("Name of the advertising platform is missing");
                continue;
            }

            // Получение списка локаций
            var locations = line[(colonIndex + 1)..]
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            // Логирование строки, если не указаны локации для платформы
            if (locations.Count == 0)
            {
                _logger.LogWarning("Site locations are missing");
                continue;
            }

            // Проверка на повторяющиеся платформы 
            if (checkPlatforms.Contains(adPlatform))
            {
                _logger.LogWarning("{lines}: The ad platform {adPlatform} is already registered", lines, adPlatform);
                continue;
            }

            // Вносим считанную платформу в список
            checkPlatforms.Add(adPlatform);

            // Проверяем, есть ли совпадение вновь считанных платформ с уже прочитанными
            var registeredLocation = checkLocations.Intersect(locations).ToList();
            if (registeredLocation.Count != 0)
            {
                foreach (var location in registeredLocation)
                {
                    _logger.LogWarning("{lines}: The location {location} is already registered", lines, location);
                }

                continue;
            }

            // Перебираем все локации и формируем словарь,
            // где ключи - локации, а значения - их платформы
            foreach (var location in locations)
            {
                // Добавляем платформу в список уже прочитанных для проверки на повторяемость
                checkLocations.Add(location);
                // Если такой локации еще нет, то создаем ключ с ней и
                // предоставляем ей пустой HashSet в качестве значения
                if (!adPlatforms.TryGetValue(location, out var value))
                {
                    value = [];
                    adPlatforms[location] = value;
                }

                // Если платформа уже есть, добавляем значения
                value.Add(adPlatform);
            }
        }

        // Дополнение списка локаций их "родительскими" локациями
        var processedAdPlatforms = ProcessDictionary(adPlatforms);

        return processedAdPlatforms;
    }

    [GeneratedRegex(@"^\s*([А-Яа-я\s\w._-]+)\s*:\s*((/([\w_-],\s+|[\w_-])+)+\s*)$")]
    private static partial Regex CheckPlatformLocationRegex();
}