using Aggregator.AuxiliaryServices;
using Microsoft.AspNetCore.Mvc;

namespace TVSeriesAgregator;
[ApiController]
[Route("api/v1/[controller]")]
public class FastUpdateFetch : ControllerBase
{
    private readonly PeriodicTask _backgroundServie;
    private readonly ILogger<FastUpdateFetch> _logger;
    public FastUpdateFetch(ILogger<FastUpdateFetch> logger, PeriodicTask periodicTask)
    {
        _backgroundServie = periodicTask;
        _logger = logger;
    }
    [HttpGet]
    public IActionResult GetUpdates()
    {
        _logger.LogInformation($"Local updates requested at {DateTime.Now}");
        if (_backgroundServie.Animes is null)
            return Problem("No updates recieved yet, pls try again later!");
        return Ok(_backgroundServie.Animes);
    }
}
