using System.Collections;
using Aggregator.DataFetchers;
using Aggregator.DataStructs;
namespace Aggregator.AuxiliaryServices;

public class PeriodicTask : BackgroundService
{
    private readonly ILogger<PeriodicTask> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private IEnumerable<Anime> _animes;
    private int _execCount = 0;
    public bool IsEnabled { get; set; }
    public IEnumerable<Anime> Animes { get { return _animes; }}
    public PeriodicTask(ILogger<PeriodicTask> logger, IServiceScopeFactory factory,IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = factory;
        IsEnabled = configuration.GetValue<bool>("IsBackground");
        CollectInfo();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);

        while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                if (IsEnabled)
                {
                    await CollectInfo();
                    _execCount++;
                    _logger.LogInformation(
                        $"Executed periodic fetching. Count: {_execCount}"
                    );
                }
                if (!IsEnabled)
                {
                    _logger.LogInformation("Skiped periodic fetching");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed to execute periodic task: {ex.Message}"
                );
            }
        }
    }
    private async Task CollectInfo()
    {
        await using AsyncServiceScope asyncscope = _scopeFactory.CreateAsyncScope();
        Anilibria anilibriaService = asyncscope.ServiceProvider.GetRequiredService<Anilibria>();
        Animevost animevostService = asyncscope.ServiceProvider.GetRequiredService<Animevost>();
        var taskList = new List<Task<IEnumerable<Anime>>>
        {
            animevostService.GetUpdates(),
            anilibriaService.GetUpdates(30)
        };
        await Task.WhenAll(taskList);
        _animes = taskList.SelectMany(task => task.Result);
        int i=0;
        foreach (Anime item in _animes)
        {
            item.Id=i;
            i++;
        }
            
        
    }
}
