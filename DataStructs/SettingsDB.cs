namespace Aggregator.DataStructs;
/// <summary>
/// Represents a structure for storing database settings
/// </summary>
public class SettingsDB
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string ListCollectionName { get; set; } = null!;
    public string SourcesCollectionName {get;set;}=null!;

}