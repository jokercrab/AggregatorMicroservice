using Microsoft.AspNetCore.Mvc;
using Aggregator.AuxiliaryServices;
namespace Aggregator.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
public class BackgroundController : ControllerBase
{
    private readonly ILogger<BackgroundController> _logger;
    private readonly PeriodicTask _backTask;

    public BackgroundController(ILogger<BackgroundController> logger,
                                PeriodicTask periodicTask)
    {
        _logger = logger;
        _backTask = periodicTask;
    }

    [HttpGet]
    public IActionResult GetState(){
        return Ok(_backTask.IsEnabled);
    }
    [HttpPatch]
    public IActionResult SetState([FromQuery] bool isEnabled){
        _backTask.IsEnabled = isEnabled;
        return Accepted($"Background status is {_backTask.IsEnabled}");
    }
}
