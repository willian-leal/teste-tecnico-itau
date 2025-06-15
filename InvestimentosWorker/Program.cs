using Confluent.Kafka;
using InvestimentosApi.Data;
using InvestimentosWorker;
using InvestimentosWorker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<InvestimentosDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddHostedService<CotacaoConsumerWorker>();
builder.Services.AddSingleton<KafkaCotacaoConsumer>();
builder.Services.AddHttpClient();

var app = builder.Build();
app.Run();