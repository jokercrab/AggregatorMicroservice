using Microsoft.AspNetCore.Mvc;
using Aggregator.DataFetchers;
using Aggregator.DataStructs;

using TVSeriesAgregator.DataStructs;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.ObjectPool;

namespace Aggregator.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FetchUpdatesController : ControllerBase
{

    private readonly ILogger<FetchUpdatesController> _logger;
    private readonly IEnumerable<IDataFetcher> _fetchers;
    public FetchUpdatesController(ILogger<FetchUpdatesController> logger, IEnumerable<IDataFetcher> fetchers)
    {
        _logger = logger;
        _fetchers = fetchers;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    public async Task<IActionResult> GetUpdates() 
    {
        _logger.LogInformation(
            "Updates requested from {RemoteIpAddress}",
            HttpContext.Connection);
        IAsyncEnumerable<Anime> mergedStream;
        try
        {
            using var fetcherEnum = _fetchers.GetEnumerator();
            var streams = new IAsyncEnumerable<Anime>[_fetchers.Count()];
            for (int i = 0; fetcherEnum.MoveNext(); i++)
                streams[i] = fetcherEnum.Current.GetUpdates();
            mergedStream = streams.Merge();


        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while processing request {ex.Message}",HttpContext.Connection);
            return Problem("Some of the sources did not return result", statusCode: StatusCodes.Status418ImATeapot);

        }


        return Ok(mergedStream);
    }

}
