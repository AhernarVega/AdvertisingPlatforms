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

    private IEnumerable<string> GetParentKeys(string key)
    {
        var parts = key.Split('/');
        return Enumerable
            .Range(1, parts.Length - 1)
            .Select(i => string.Join("/", parts.Take(i)));
    }

    private Dictionary<string, HashSet<string>> ProcessDictionary(Dictionary<string, HashSet<string>> dict)
    {
        var result = new Dictionary<string, HashSet<string>>();

        foreach (var key in dict.Keys)
        {
            var values = new HashSet<string>(dict[key]);

            foreach (var parentKey in GetParentKeys(key))
            {
                if (dict.TryGetValue(parentKey, out var parentValues))
                {
                    values.UnionWith(parentValues);
                }
            }

            result[key] = values;
        }

        return result;
    }

    public async Task<Dictionary<string, HashSet<string>>> LoadAdPlatformsAsync(string? pathToFile)
    {
        pathToFile = string.IsNullOrEmpty(pathToFile)
            ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\AdPlatforms.txt")
            : pathToFile;

        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException($"{Path.GetFileName(pathToFile)} file not found");
        }

        var adPlatforms = new Dictionary<string, HashSet<string>>();

        using var reader = new StreamReader(pathToFile);

        var checkPlatforms = new List<string>();
        var checkLocations = new List<string>();
        var lines = 0;

        while (await reader.ReadLineAsync() is { } line)
        {
            ++lines;

            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            
            var regex = CheckPlatformLocationRegex();
            if (!regex.Match(line).Success)
            {
                _logger.LogWarning($"{lines}: The string had an incorrect format. Expected format:" +
                                   $" [PlatformName]: [/location1], [/location2], .. [/locationN]");
            }
            
            var colonIndex = line.IndexOf(':');
            if (colonIndex == -1)
            {
                _logger.LogWarning("The \":\" is missing");
                continue;
            }

            var adPlatform = line[..colonIndex];
            if (string.IsNullOrEmpty(adPlatform))
            {
                _logger.LogWarning("Name of the advertising platform is missing");
                continue;
            }

            var locations = line[(colonIndex + 1)..]
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (locations.Count == 0)
            {
                _logger.LogWarning("Site locations are missing");
                continue;
            }

            if (checkPlatforms.Contains(adPlatform))
            {
                _logger.LogWarning("{lines}: The ad platform {adPlatform} is already registered", lines, adPlatform);
                continue;
            }

            checkPlatforms.Add(adPlatform);

            var registeredLocation = checkLocations.Intersect(locations).ToList();
            if (registeredLocation.Count != 0)
            {
                foreach (var location in registeredLocation)
                {
                    _logger.LogWarning("{lines}: The location {location} is already registered", lines, location);
                }
                continue;
            }

            foreach (var location in locations)
            {
                checkLocations.Add(location);
                if (!adPlatforms.TryGetValue(location, out var value))
                {
                    value = [];
                    adPlatforms[location] = value;
                }

                value.Add(adPlatform);
            }
        }

        var processedAdPlatforms = ProcessDictionary(adPlatforms);

        return processedAdPlatforms;
    }

    [GeneratedRegex(@"^\s*([А-Яа-я\s\w._-]+)\s*:\s*((/([\w_-],\s+|[\w_-])+)+\s*)$")]
    private static partial Regex CheckPlatformLocationRegex();
}