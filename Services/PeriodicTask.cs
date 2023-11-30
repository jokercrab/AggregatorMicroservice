using System.Collections;
using Aggregator.DataFetchers;
using Aggregator.DataStructs;
using System.Diagnostics;

using Microsoft.AspNetCore.Identity;
namespace Aggregator.Services;
/// <summary>
/// Periodic service for background updating
/// </summary>
public class PeriodicTask : BackgroundService
{
    private readonly ILogger<PeriodicTask> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(1);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDataBase _data;
    private int _execCount = 0;
    public bool IsEnabled { get; set; }
    public PeriodicTask(ILogger<PeriodicTask> logger, IServiceScopeFactory factory, IConfiguration configuration,
                        IDataBase dataBase)
    {
        _logger = logger;
        _scopeFactory = factory;
        _data = dataBase;
        IsEnabled = configuration.GetValue<bool>("IsBackground");
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);
        do
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
                _logger.LogError(
                    $"Failed to execute periodic task: {ex.Message}",
                    ex
                );
            }

        } while (
            !stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken));
    }
    private async Task CollectInfo()
    {
        await using AsyncServiceScope asyncscope = _scopeFactory.CreateAsyncScope();
        IEnumerable<IDataFetcher> services = asyncscope.ServiceProvider.GetServices<IDataFetcher>();

        IAsyncEnumerable<Anime> mergedStream;
        // Start fetching data from all of the sources
        using (var serviceEnum = services.GetEnumerator())
        {
            var tasks = new IAsyncEnumerable<Anime>[services.Count()];
            for (int i = 0; serviceEnum.MoveNext(); i++)
                tasks[i] = serviceEnum.Current.GetUpdates();

            // Merge result into one stream
            mergedStream = tasks.Merge();
            
        }
        // Process each record when ready
        await foreach (var result in mergedStream)
        {
            try
            {
                if (await _data.Exists(result.CodeName))
                    await _data.Update(result);
                else
                    await _data.InsertOne(result);
            }
            catch (Exception ex)
            {

                _logger.LogError(
                    $"Failed to update DB collection: {ex.Message}",
                    ex
                );
            }


        }





    }
}
