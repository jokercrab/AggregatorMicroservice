using Aggregator.DataStructs;
namespace Aggregator.Services;

public interface IDataBase
{
    public Task<List<Anime>> Get();
    public Task InsertMany(IEnumerable<Anime> animes);
    public Task InsertOne(Anime anime);
    public Task<bool> Exists(string codeName);
    public Task<bool> Update(Anime anime);
}
