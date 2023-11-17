using MongoDB.Driver;
using Aggregator.DataStructs;

using Microsoft.Extensions.Options;
namespace Aggregator.Services;
/// <summary>
/// Used to interact with mongo database
/// </summary>
public class MongoHelper : IDataBase
{
    private IMongoCollection<Anime> _animeCollection;
    private IMongoDatabase database;
    public MongoHelper(
        IOptions<SettingsDB> DatabaseSettings)
    {
        var clientt = new MongoClient(
            DatabaseSettings.Value.ConnectionString);

        database = clientt.GetDatabase(
            DatabaseSettings.Value.DatabaseName);

        _animeCollection = database.GetCollection<Anime>(
            DatabaseSettings.Value.ListCollectionName);
    }

    public async Task<List<Anime>> Get() => await _animeCollection.Find(_ => true).ToListAsync();
    public async Task InsertMany(IEnumerable<Anime> animes) => await _animeCollection.InsertManyAsync(animes);
    public async Task InsertOne(Anime anime) => await _animeCollection.InsertOneAsync(anime);
    public async Task<bool> Exists(string codeName) => await _animeCollection.Find<Anime>(a => a.CodeName == codeName).AnyAsync();
    public async Task<bool> Replace(Anime anime)
    {
        var filter = Builders<Anime>.Filter.Eq(a => a.CodeName, anime.CodeName);
        var result = await _animeCollection.ReplaceOneAsync(filter, anime);
        return result.IsAcknowledged;
    }
    public async Task<bool> Update(Anime anime)
    {
        var update = Builders<Anime>.Update
            .Set(a=>a.Poster,anime.Poster)
            .Set(a=>a.Updated,anime.Updated)
            .Set(a=>a.Episodes,anime.Episodes);
        var filter = Builders<Anime>.Filter.Eq(a => a.CodeName, anime.CodeName);
        var result = await _animeCollection.UpdateOneAsync(filter,update);
        return result.IsAcknowledged;
    }


}