using System.Text.Json.Nodes;
using System.Text.Json;
using Aggregator.DataStructs;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections;
using System.Net.Http.Headers;

namespace Aggregator.DataFetchers;

public class Anilibria
{
    private readonly HttpClient _httpClient;
    private const string _baseURI = "https://api.anilibria.tv/";
    public Anilibria(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_baseURI);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Anything");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<IEnumerable<Anime>?> GetUpdates(int limit)
    {
        JsonNode? json;
        var timestamp = DateTimeOffset.Now.AddDays(-7).ToUnixTimeSeconds();
        var query = QueryHelpers.AddQueryString(
            "/v3/title/updates",
            new Dictionary<string, string?>{
                {"filter", "code,names,posters,status,type,genres,player.episodes,updated"},
                {"since", timestamp.ToString()},
                {"limit",limit.ToString()}
            });
        try
        {
            var response = await _httpClient.GetAsync(query);

            var node = JsonNode.Parse(response.Content.ReadAsStringAsync().Result);
            json = node!["list"];

        }
        catch (Exception ex)
        {
            throw new Exception($"{ex.Message}\n AnilibriaUpdates.GetUpdates");
        }

        if (json == null)
            return null;
        var jsonResult = json.Deserialize<AnilibraiDataStruct[]>
        (new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if(jsonResult == null)
            return null;
        var result = new Anime[jsonResult.Length];
        for(int i=0;i<result.Length;i++)
        {
            result[i] = new Anime(jsonResult[i]);
        }
        return result;
    }
    private DateTime SecondsToDateTime(long timestamp, bool local = false)
    {
        var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return local ? offset.LocalDateTime : offset.UtcDateTime;
        
    }


}