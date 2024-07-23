using Discord.WebSocket;
using Discord;
using Beelzebot.webapi.Services;

namespace Beelzebot.webapi
{
    public class DiscordBotService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly ILogger<DiscordBotService> _logger;
        private readonly IBeelzebotInteractions _beelzebotInteractions;

        private readonly string _token ;

        public DiscordBotService(ILogger<DiscordBotService> logger, IBeelzebotInteractions beelzebotInteractions, string token)
        {
            _beelzebotInteractions = beelzebotInteractions;
            _client = new DiscordSocketClient();
            _logger = logger;
            _token = token;

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            _logger.LogInformation("Discord bot started");

            await Task.Delay(-1, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Discord bot stopping");

            return _client.LogoutAsync();
        }

        private Task LogAsync(LogMessage log)
        {
            _logger.LogInformation(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            _logger.LogInformation("Discord bot is connected!");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            var response = await _beelzebotInteractions.GetResponse(message.Content.ToLower());

            await message.Channel.SendMessageAsync(response);
        }

        public async Task SendMessageToChannelAsync(ulong channelId, string message)
        {
            var channel = _client.GetChannel(channelId) as IMessageChannel;
            if (channel != null)
            {
                await channel.SendMessageAsync(message);
            }
            else
            {
                _logger.LogError($"Channel with ID {channelId} not found.");
            }
        }
    }
}
