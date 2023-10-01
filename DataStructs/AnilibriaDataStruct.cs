using System.Text.Json.Serialization;

namespace Aggregator.DataStructs;

public class AnilibraiDataStruct : IEquatable<AnilibraiDataStruct>
{

    public Posters Posters { get; set; }
    public Type Type { get; set; }
    public Status Status { get; set; }
    public string Code { get; set; }
    public Names Names { get; set; }
    public IEnumerable<string>? Genres { get; set; }
    public Player Player { get; set; }
    public int Updated { get; set; }


    public bool Equals(AnilibraiDataStruct? other)
    {
        return Code == other?.Code;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as AnilibraiDataStruct);
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }
}
public class Names
{
    public string? Ru { get; set; }
    public string? En { get; set; }
    public string? Alternative { get; set; }
}

public class PosterOriginal
{
    public string? Url { get; set; }
    public object? Raw_base64_file { get; set; }
}
public class PosterSmall : PosterOriginal
{

}
public class PosterMedium : PosterOriginal
{

}


public class Posters
{
    public PosterSmall Small { get; set; }
    public PosterMedium Medium { get; set; }
    public PosterOriginal Original { get; set; }

}
public class Status
{
    [JsonPropertyName("string")]
    public string? Name { get; set; }
    public int? Code { get; set; }
}
public class Player
{
    public EpisodesInfo Episodes { get; set; }
}
public class EpisodesInfo
{
    public int First { get; set; }
    public int Last { get; set; }
    [JsonPropertyName("string")]
    public string FulString { get; set; }
    public EpisodesInfo() { }
    public EpisodesInfo(string fullInfo)
    {
        try
        {
            var splited = fullInfo.Split('-');
            First = int.Parse(splited[0]);
            Last = int.Parse(splited[1]);
        }
        catch(Exception){}
        finally
        {
            FulString = fullInfo;

        }

    }
}

public class Type
{
    [JsonPropertyName("full_string")]
    public string? FullString { get; set; }
    public int Code { get; set; }
    [JsonPropertyName("string")]
    public string? ShortString { get; set; }
    public int? Episodes { get; set; }
    public int? Length { get; set; }
}
