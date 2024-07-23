using UpdatePublicAddress.Contracts;
using MassTransit;


namespace Beelzebot.webapi.Consumers
{
    public class IPAddressUpdateMessageConsumer : IConsumer<IPAddressUpdateMessage>
    {
        private readonly ILogger<IPAddressUpdateMessageConsumer> _logger;
        private readonly DiscordBotService _discordBotService;

        public IPAddressUpdateMessageConsumer(ILogger<IPAddressUpdateMessageConsumer> logger, DiscordBotService discordBotService)
        {
            _logger = logger;
            _discordBotService = discordBotService;
        }

        public Task Consume(ConsumeContext<IPAddressUpdateMessage> context)
        {
            _logger.LogInformation($"Message consumed: {context.Message.IPAddress}");

            ulong channelId = 1264954011061059584;

            string message = $"IP address at home: {context.Message.IPAddress}";

            return _discordBotService.SendMessageToChannelAsync(channelId, message);
        }
    }
}
