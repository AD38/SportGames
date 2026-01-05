using SportGames.Core.Interfaces;
using SportGames.Core.Models;

namespace SportGames.Collector;

public class Worker(
    ISourceLeaseStore sourceLeaseStore,
    IEnumerable<IGameDataRetrievingService> gameDataRetrievingServices,
    IGameRepository gameRepository,
    IDeduplicationCache deduplicationCache) : BackgroundService
{
    private readonly string _workerId = Environment.MachineName;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        //use IOptions to set ttl and delay
        var leaseTtl = TimeSpan.FromSeconds(60);
        var pollDelay = TimeSpan.FromSeconds(20);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Parallel.ForEachAsync(
                gameDataRetrievingServices,
                new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = Environment.ProcessorCount, //use IOptions to set
                },
                async (source, ct) =>
                {
                    var acquired = await sourceLeaseStore.TryAcquireAsync(
                        source.SourceId,
                        _workerId,
                        leaseTtl,
                        ct);

                    if (!acquired)
                    {
                        return;
                    }

                    using var leaseRenewal = StartLeaseRenewal(source.SourceId, leaseTtl, ct);

                    try
                    {
                        await ProcessSourceAsync(source, ct);
                    }
                    catch (Exception ex)
                    {
                        //log exception
                    }
                    finally
                    {
                        await leaseRenewal.CancelAsync();
                        await sourceLeaseStore.ReleaseAsync(source.SourceId, _workerId, ct);
                    }
                });

            await Task.Delay(pollDelay, cancellationToken);
        }
    }

    private async Task ProcessSourceAsync(
        IGameDataRetrievingService source,
        CancellationToken ct)
    {
        var games = await source.GetGames(ct);
        var newGames = new List<Game>();

        foreach (var game in games)
        {
            var key = game.GetNormalizedKey();

            if (await deduplicationCache.TryAdd(key, ct))
            {
                newGames.Add(game);
            }
        }

        await gameRepository.Save(newGames, ct);
    }

    private CancellationTokenSource StartLeaseRenewal(
        string sourceId,
        TimeSpan leaseTtl,
        CancellationToken cancellationToken)
    {
        var renewalInterval = TimeSpan.FromSeconds(leaseTtl.TotalSeconds / 2);

        var renewalCancellationToken = new CancellationTokenSource();
        var linkedCancellationToken = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken, renewalCancellationToken.Token);

        _ = Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(renewalInterval);

            try
            {
                while (await timer.WaitForNextTickAsync(linkedCancellationToken.Token))
                {
                    var renewed = await sourceLeaseStore.RenewAsync(
                        sourceId,
                        _workerId,
                        leaseTtl,
                        linkedCancellationToken.Token);

                    if (!renewed)
                        break;
                }
            }
            finally
            {
                linkedCancellationToken.Dispose();
            }
        }, cancellationToken);

        return renewalCancellationToken;
    }
}