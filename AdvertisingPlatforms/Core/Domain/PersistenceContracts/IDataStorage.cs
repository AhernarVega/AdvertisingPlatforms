namespace AdvertisingPlatforms.Core.Domain.PersistenceContracts;

public interface IDataStorage
{
    public void UpdateData(Dictionary<string, HashSet<string>> adPlatforms);
    public Dictionary<string, HashSet<string>> GetAdPlatforms();
}