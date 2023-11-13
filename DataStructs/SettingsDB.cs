namespace Aggregator.DataStructs;

public class SettingsDB
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ListCollectionName { get; set; } = null!;
    public string SourcesCollectionName {get;set;}=null!;

}