using Microsoft.AspNetCore.Mvc;
using Aggregator.DataFetchers;
using Aggregator.DataStructs;
using TVSeriesAgregator.DataStructs;
using System.Diagnostics;

namespace Aggregator.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FetchUpdatesController : ControllerBase
{

    private readonly ILogger<FetchUpdatesController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly Anilibria _anilibriaService;
    private readonly Animevost _animevostService;
    public FetchUpdatesController(ILogger<FetchUpdatesController> logger, IHttpClientFactory client)
    {
        _logger = logger;
        _clientFactory = client;
        _anilibriaService = new Anilibria(_clientFactory.CreateClient());
        _animevostService = new Animevost(_clientFactory.CreateClient());
    }

    [HttpGet]   
    [ProducesResponseType(StatusCodes.Status418ImATeapot)]
    public async Task<IActionResult> GetUpdates()
    {
        _logger.LogInformation(
            "Updates requested from {RemoteIpAddress}",
            HttpContext.Connection);
        var watch = new Stopwatch();
        //####################################
        _logger.LogInformation(
            "Started fetching from anilibria",
            HttpContext.Connection);
        watch.Start();
        var anilibria = await _anilibriaService.GetUpdates(30);
        watch.Stop();
        _logger.LogInformation(
                $"Finished fetching from anilibria time:{watch.ElapsedMilliseconds}",
                HttpContext.Connection);
        //###################################

        _logger.LogInformation(
            "Started fetching from animevost",
            HttpContext.Connection);
        _logger.LogInformation(
            "Started fetching from animevost",
            HttpContext.Connection);
        watch.Start();
        var animevost = await _animevostService.GetUpdates();
        watch.Stop();
        _logger.LogInformation(
            $"Finished fetching from animevost time:{watch.ElapsedMilliseconds}",
            HttpContext.Connection);
        //############################################
        if (anilibria != null && animevost != null)
            return Ok(anilibria.Concat(animevost));
        return Problem("Some of the sources did not return result", statusCode: StatusCodes.Status418ImATeapot);

    }

}
