using System.Collections;
using Aggregator;
using Aggregator.DataStructs;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;

namespace Aggregator.DataFetchers;

public partial class Animevost
{
    private readonly HttpClient _httpClient;
    private const string _baseURI = "https://animevost.org";
    private HtmlWeb _htmlWeb;
    [GeneratedRegex("(?<=\\/\\s)[A-z].*?(?=\\s\\[)")]
    private static partial Regex NameEN();
    [GeneratedRegex("[А-я].*?(?=\\s\\/)")]
    private static partial Regex NameRU();
    [GeneratedRegex("(?<=\\[)(.*?)(?=])")]
    private static partial Regex About();
    [GeneratedRegex("(?<=\\[)\\d+-\\d+")]
    private static partial Regex Episodes();
    [GeneratedRegex("([^\\/]+(?=\\.html))")]
    private static partial Regex Code();

    public Animevost(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_baseURI);
        _htmlWeb = new HtmlWeb();
    }
    private async Task<IList<string>> GetLinks()
    {
        var page = await _htmlWeb.LoadFromWebAsync(_baseURI);
        var node = page.DocumentNode.SelectSingleNode("//ul[@class='raspis raspis_fixed']");
        List<string> refs = new();
        foreach (var children in node.ChildNodes)
        {
            if (children.Name == "li")
            {
                var record = children.FirstChild;

                refs.Add(record.Attributes["href"].Value);
            }

        }
        return refs;
    }
    public async Task<Anime> ExtractInfo(string link)
    {
        var anime = new Anime();
        var page = await _htmlWeb.LoadFromWebAsync(link);
        var node = page.DocumentNode.SelectSingleNode("//h1");
        anime.Names = new Names
        {
            En = NameEN().Match(node.InnerText).Value, 
            Ru = NameRU().Match(node.InnerText).Value
        };
        
        anime.About = About().Match(node.InnerText).Value;
        var episodesInfo = Episodes().Match(node.InnerText).Value;
        anime.Episodes = new EpisodesInfo(episodesInfo);
        node=page.DocumentNode.SelectSingleNode("/html/body/div/div[3]/div[2]/div[2]/div[1]/div[2]/div[1]/span[2]");
        ///html/body/div/div[3]/div[2]/div[2]/div[1]/div[2]/div[1]/span[2]
        string dateRu = node.InnerText;
        anime.Updated =  DateTimeOffset.Parse(dateRu, new CultureInfo("ru-RU"));
        node = page.DocumentNode.SelectSingleNode("//img[@class='imgRadius']");
        anime.Poster = node.Attributes["src"].Value;
        anime.CodeName = Code().Match(link).Value;
        anime.Host = "https://animevost.org/";
        anime.Source = "Animevost";

        return anime;
    }
    public async Task<IEnumerable<Anime>> GetUpdates()
    {
        var links = await GetLinks();
        //Anime[] animes = new Anime[links.Count];
        var taskList = new Task<Anime>[links.Count];
        for(int i=0;i<links.Count;i++){
            taskList[i] = ExtractInfo(links[i]);
        }

        return await Task.WhenAll(taskList);
    }

}