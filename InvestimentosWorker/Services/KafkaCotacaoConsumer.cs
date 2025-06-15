using Confluent.Kafka;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InvestimentosWorker.Services;

public class KafkaCotacaoConsumer
{
    private readonly ILogger<KafkaCotacaoConsumer> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public KafkaCotacaoConsumer(
        ILogger<KafkaCotacaoConsumer> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "cotacao-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("cotacoes");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                var message = result.Message.Value;

                _logger.LogInformation($"Mensagem recebida: {message}");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var cotacao = JsonSerializer.Deserialize<CotacaoDto>(message, options);

                if (cotacao == null)
                {
                    _logger.LogWarning("Cotação inválida.");
                    continue;
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync("http://localhost:5157/cotacoes", cotacao);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Cotação enviada com sucesso para a API.");
                }
                else
                {
                    _logger.LogWarning($"Erro ao enviar cotação: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Detalhes do erro: {errorContent}");
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError($"Erro de consumo: {ex.Error.Reason}");
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro geral no processamento: {ex.Message}");
                await Task.Delay(1000);
            }
        }

        consumer.Close();
    }

    public class CotacaoDto
    {
        [JsonPropertyName("ativoId")]
        public int AtivoId { get; set; }

        [JsonPropertyName("precoUnitario")]
        public decimal PrecoUnitario { get; set; }

        [JsonPropertyName("dataHora")]
        public DateTime DataHora { get; set; }
    }
}
