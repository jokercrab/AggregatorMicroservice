using Aggregator.Services;
using Aggregator.DataStructs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TVSeriesAgregator;
[ApiController]
[Route("api/v1/[controller]")]
public class FastUpdateFetch : ControllerBase
{
    private readonly PeriodicTask _backgroundServie;
    private readonly ILogger<FastUpdateFetch> _logger;
    private readonly IDataBase _dataBase;
    public FastUpdateFetch(ILogger<FastUpdateFetch> logger, PeriodicTask periodicTask, 
                            IDataBase dataBase)
    {
        _backgroundServie = periodicTask;
        _logger = logger;
        _dataBase = dataBase;
    }
    [HttpGet]
    public async Task<IActionResult> GetUpdates()
    {
        _logger.LogInformation($"Local updates requested at {DateTime.Now}");
        try
        {
            var animeList = await _dataBase.Get();
            return Ok(animeList);

        }catch(Exception ex){
            _logger.LogError($"Unexpected exception happend at FastUpdateFetch.GetUpdates: {ex.Message}",ex);
            return NotFound();
        }
    }
}
