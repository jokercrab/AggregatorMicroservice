using System.Collections;
using Aggregator;
using Aggregator.DataStructs;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace Aggregator.DataFetchers;

public partial class Animevost : IDataFetcher
{
    private const string _baseURI = "https://animevost.org";
    private HtmlWeb _htmlWeb;
    // regex for EN name fetching
    [GeneratedRegex("(?<=\\/\\s)[A-z].*?(?=\\s\\[)")]
    private static partial Regex NameEN();
    //
    // regex for RU name fetching
    [GeneratedRegex("[А-я].*?(?=\\s\\/)")]
    private static partial Regex NameRU();
    //
    // regex for fetching description
    [GeneratedRegex("(?<=\\[)(.*?)(?=])")]
    private static partial Regex About();
    //
    // regex for fetching episode coount
    [GeneratedRegex("(?<=\\[)\\d+-\\d+")]
    private static partial Regex Episodes();
    //
    // regex for fetching code names
    [GeneratedRegex("([^\\/]+(?=\\.html))")]
    private static partial Regex Code();

    public Animevost()
    {
        _htmlWeb = new HtmlWeb();
    }
    /// <summary>
    /// Gets a list of http references for new realeses from Animevost.org
    /// </summary>
    /// <returns>List of references</returns>
    private async Task<IList<string>> GetLinks()
    {
        var page = await _htmlWeb.LoadFromWebAsync(_baseURI);
        // Select list of new releases in html
        var node = page.DocumentNode.SelectSingleNode("//ul[@class='raspis raspis_fixed']");
        // Generete a list of references for new releses
        List<string> refs = new();
        foreach (var children in node.ChildNodes)
        {
            if (children.Name == "li") // If children is list elemnt
            {
                // Get next child and fetch reference
                var record = children.FirstChild;
                refs.Add(record.Attributes["href"].Value);
            }

        }
        return refs;
    }
    /// <summary>
    /// Exctracts info from given Animevost page
    /// </summary>
    /// <param name="link">Http reference to the page</param>
    /// <returns>New Anime object constructed from given page</returns>
    private async Task<Anime> ExtractInfo(string link)
    {
        var anime = new Anime();
        // Load page
        var page = await _htmlWeb.LoadFromWebAsync(link);
        // Select header
        var node = page.DocumentNode.SelectSingleNode("//h1");
        // Fetch needed info
        anime.Names = new Names
        {
            En = NameEN().Match(node.InnerText).Value,
            Ru = NameRU().Match(node.InnerText).Value
        };
        anime.About = About().Match(node.InnerText).Value;
        var episodesInfo = Episodes().Match(node.InnerText).Value;
        anime.Episodes = new EpisodesInfo(episodesInfo);
        node = page.DocumentNode.SelectSingleNode("/html/body/div/div[3]/div[2]/div[2]/div[1]/div[2]/div[1]/span[2]");
        string dateRu = node.InnerText;
        anime.Updated = DateTimeOffset.Parse(dateRu, new CultureInfo("ru-RU"));
        node = page.DocumentNode.SelectSingleNode("//img[@class='imgRadius']");
        anime.Poster = node.Attributes["src"].Value;
        anime.CodeName = Code().Match(link).Value;
        anime.Host = "https://animevost.org/";
        anime.Source = "Animevost";

        return anime;
    }
    /// <summary>
    /// Gets all recent updates from animevost
    /// </summary>
    /// <returns>A list of newly constructed Anime objects</returns>
    public async IAsyncEnumerable<Anime> GetUpdates()
    {
        var links = await GetLinks();
        for (int i = 0; i < links.Count; i++)
        {
            yield return await ExtractInfo(links[i]);
        }

    }

}