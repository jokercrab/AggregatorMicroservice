using Microsoft.AspNetCore.Mvc;
using Aspnet_Back.Datafetchers;
namespace Aspnet_Back.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AnilibriaUpdatesController : ControllerBase
{

    private readonly ILogger<AnilibriaUpdatesController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly Anilibria _anilibriaService;
    public AnilibriaUpdatesController(ILogger<AnilibriaUpdatesController> logger, IHttpClientFactory client)
    {
        _logger = logger;
        _clientFactory = client;
        _anilibriaService = new Anilibria(_clientFactory.CreateClient());
        
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetUpdates()
    {
        _logger.LogInformation(
            "Updates requested from {RemoteIpAddress}",
            HttpContext.Connection);
        var anilibria = await _anilibriaService.GetUpdates(30);
        return Ok(anilibria);
    }

}
