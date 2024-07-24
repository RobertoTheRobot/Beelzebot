using Beelzebot.webapi.Consumers;
using Beelzebot.webapi;
using MassTransit;
using System.Runtime.CompilerServices;
using Beelzebot.webapi.Services;
using Beelzebot.webapi.Queries;

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

//string serviceBusConnectionString = builder.Configuration["ServiceBusConnectionString"];
string discordBotToken = builder.Configuration["DiscordBotToken"];  // Get the Discord bot token from the configuration
string openApiApiKey = builder.Configuration["OpenAIApiKey"];

builder.Services.AddSingleton(provider =>
    new DiscordBotService(
        provider.GetRequiredService<ILogger<DiscordBotService>>(),
        provider.GetRequiredService<IBeelzebotInteractions>(),
        discordBotToken
    )
);

builder.Services.AddHostedService(provider => provider.GetRequiredService<DiscordBotService>());

// Add other services

builder.Services.AddTransient<IBeelzebotInteractions, BeelzebotInteractions>();
builder.Services.AddTransient<IGetPublicIPQuery, GetPublicIPQuery>();

// Register OpenAIService with dependencies
builder.Services.AddTransient<IOpenAIService>(provider =>
{    
    return new OpenAIService(openApiApiKey, provider.GetRequiredService<ILogger<OpenAIService>>());
});


//builder.Services.AddMassTransit(x =>
//{
//    // Consumers
//    x.AddConsumer<IPAddressUpdateMessageConsumer>();

//    // Transport
//    x.UsingAzureServiceBus((context, cfg) =>
//    {
//        cfg.UseServiceBusMessageScheduler();
//        cfg.Host(serviceBusConnectionString);

//        cfg.ReceiveEndpoint("ip-address-queue", e =>
//        {
//            e.ConfigureConsumeTopology = false;
//            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(30)));
//            e.ConfigureConsumer<IPAddressUpdateMessageConsumer>(context);
//        });

//    });
//});


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
