using InvestimentosWorker.Services;

namespace InvestimentosWorker;

public class CotacaoConsumerWorker : BackgroundService
{
    private readonly KafkaCotacaoConsumer _consumer;

    public CotacaoConsumerWorker(KafkaCotacaoConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync(stoppingToken);
    }
}