using Aggregator.DataStructs;
namespace Aggregator.DataFetchers;

public interface IDataFetcher
{
    public IAsyncEnumerable<Anime> GetUpdates();
}
