namespace AdvertisingPlatforms.Infrastructure.Persistence;

public class AdPlatformsReader
{
    public async Task<Dictionary<string, List<string>>> LoadAdPlatformsAsync(string pathToFile = "")
    {
        if (string.IsNullOrEmpty(pathToFile))
        {
            pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\AdPlatforms.txt");
        }

        if (!File.Exists(pathToFile))
        {
            throw new FileNotFoundException($"{Path.GetFileName(pathToFile)} file not found");
        }

        var adPlatforms = new Dictionary<string, List<string>>();

        using var reader = new StreamReader(pathToFile);
        
        while (await reader.ReadLineAsync() is { } line)
        {
            var adPlatform = line[..line.IndexOf(':')];
            var locations = line[(line.IndexOf(':') + 1)..]
                .Split(",")
                .Select(x => x.Trim())
                .ToList();

            if (locations.Count == 0) continue;

            if (adPlatforms.TryGetValue(adPlatform, out var value))
            {
                value.AddRange(locations);
            }
            else
            {
                adPlatforms.TryAdd(adPlatform, locations);
            }
        }

        return adPlatforms;
    }
}