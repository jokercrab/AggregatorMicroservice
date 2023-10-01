using System;
namespace Aggregator.DataStructs;

public class Anime
{
    public int Id {get;set;}
    public Names Names { get; set; }
    public string Poster { get; set; }
    public string About { get; set; }
    public EpisodesInfo Episodes { get; set; }
    public string CodeName { get; set; }
    public string Source { get; set; }
    public string Host{ get; set; }
    public DateTimeOffset Updated { get; set; }
    public Anime(Names Names, string Poster, string About, string CodeName,
                string Source, EpisodesInfo Episodes, DateTimeOffset Updated)
    {
        this.Names = Names;
        this.Poster = Poster;
        this.About = About;
        this.CodeName = CodeName;
        this.Source = Source;
        this.Episodes = Episodes;
        this.Updated = Updated;
    }
    public Anime(AnilibraiDataStruct anilibria)
    {
        Names = anilibria.Names;
        Poster = anilibria.Posters.Medium.Url;
        About = anilibria.Type.FullString;
        Episodes = anilibria.Player.Episodes;
        Updated = DateTimeOffset.FromUnixTimeSeconds(anilibria.Updated);
        CodeName = anilibria.Code;
        Host = "https://www.anilibria.tv/";
        Source = "Anilibria";

    }
    public Anime()
    {

    }

}
