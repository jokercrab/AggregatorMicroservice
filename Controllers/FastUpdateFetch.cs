using Aggregator.Services;
using Aggregator.DataStructs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TVSeriesAgregator;
[ApiController]
[Route("api/[controller]")]
public class FastUpdateFetch : ControllerBase
{
    private readonly ILogger<FastUpdateFetch> _logger;
    private readonly IDataBase _dataBase;
    public FastUpdateFetch(ILogger<FastUpdateFetch> logger, PeriodicTask periodicTask, 
                            IDataBase dataBase)
    {
        _logger = logger;
        _dataBase = dataBase;
    }
    [HttpGet]
    public async Task<IActionResult> GetUpdates()
    {
        _logger.LogInformation($"Local updates requested at {DateTime.Now}");
        try
        {
            // Get all records from database
            var animeList = await _dataBase.Get();
            return Ok(animeList);

        }catch(Exception ex){
            _logger.LogError($"Unexpected exception happend at FastUpdateFetch.GetUpdates: {ex.Message}",ex);
            return NotFound();
        }
    }
}
