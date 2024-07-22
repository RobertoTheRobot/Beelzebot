using Beelzebot.webapi.Consumers;
using Beelzebot.webapi;
using MassTransit;
using System.Runtime.CompilerServices;
using Beelzebot.webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Configuration.AddEnvironmentVariables("Secret_");

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

string serviceBusConnectionString = builder.Configuration["ServiceBusConnectionString"];


builder.Services.AddSingleton<DiscordBotService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<DiscordBotService>());

// Add other services

builder.Services.AddTransient<IBeelzebotInteractions, BeelzebotInteractions>();


builder.Services.AddMassTransit(x =>
{
    // Consumers
    x.AddConsumersFromNamespaceContaining<IPAddressUpdateMessageConsumer>();

    // Transport
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.UseServiceBusMessageScheduler();
        cfg.Host(serviceBusConnectionString);

        cfg.ReceiveEndpoint("ip-address-queue", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(30)));
            e.ConfigureConsumer<IPAddressUpdateMessageConsumer>(context);
        });

    });


});

builder.WebHost.ConfigureKestrel((hostingContext, serverOptions) =>
{
    serverOptions.ListenAnyIP(4445);
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
