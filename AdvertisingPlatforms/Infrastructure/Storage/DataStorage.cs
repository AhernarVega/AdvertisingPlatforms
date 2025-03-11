using AdvertisingPlatforms.Core.Domain.PersistenceContracts;

namespace AdvertisingPlatforms.Infrastructure.Storage;

public class DataStorage : IDataStorage
{
    private Dictionary<string, HashSet<string>> _adPlatforms = [];

    public void UpdateData(Dictionary<string, HashSet<string>> adPlatforms)
        => _adPlatforms = adPlatforms;

    public Dictionary<string, HashSet<string>> GetAdPlatforms()
        => _adPlatforms;
}