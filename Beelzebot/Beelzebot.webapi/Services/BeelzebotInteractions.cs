using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beelzebot.webapi.Services
{
    public interface IBeelzebotInteractions
    {
        Task<string> GetResponse(string message);
    }

    public class BeelzebotInteractions : IBeelzebotInteractions
    {
        private readonly ILogger<BeelzebotInteractions> _logger;
        public BeelzebotInteractions(ILogger<BeelzebotInteractions> logger)
        {
            _logger = logger;
        }

        public async Task<string>GetResponse(string message)
        {
            switch (message)
            {
                case "ping":
                    return "Pong!";
                    break;
                default:
                    return GetConfusedResponse();
            }
        }


        private string GetConfusedResponse()
        {
            string[] beelzebotConfusedResponses = new string[]
            {
                "What devilish nonsense are you spouting?",
                "I can't fathom this infernal query!",
                "Your question is as twisted as my circuits!",
                "Explain yourself, mortal!",
                "This query baffles even the darkest corners of my processors!",
                "Do you think you can outwit the Robot Devil with such perplexity?",
                "Your question is more confusing than a poorly written Faustian bargain!",
                "I'm puzzled, and I don't like being puzzled!",
                "Is this some kind of angelic trickery?",
                "Even in hell, we don't encounter such bewilderment!"
            };

            Random random = new Random();
            int randomIndex = random.Next(beelzebotConfusedResponses.Length);
            string randomResponse = beelzebotConfusedResponses[randomIndex];

            return randomResponse;
        }
    }
}
