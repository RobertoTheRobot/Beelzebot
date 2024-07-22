using UpdatePublicAddress.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beelzebot.application.Commands
{
    public class IPAddressUpdateMessageConsumer : IConsumer<IPAddressUpdateMessage>
    {
        private readonly ILogger<IPAddressUpdateMessageConsumer> _logger;

        public IPAddressUpdateMessageConsumer(ILogger<IPAddressUpdateMessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<IPAddressUpdateMessage> context)
        {
            _logger.LogInformation($"Message consumed: {context.Message.IPAddress}");
            return Task.CompletedTask;
        }
    }
}
