using Beelzebot.webapi.Queries;
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
        private readonly IGetPublicIPQuery _getPublicIPQuery;
        private readonly IOpenAIService _openAIService;


        public BeelzebotInteractions(ILogger<BeelzebotInteractions> logger, IGetPublicIPQuery getPublicIPQuery, IOpenAIService openAIService)
        {
            _logger = logger;
            _getPublicIPQuery = getPublicIPQuery;
            _openAIService = openAIService;
        }

        public async Task<string>GetResponse(string message)
        {
            _logger.LogInformation("Received message: {Message}", message);

            if (message.StartsWith("ask:"))
            {
                string question = message.Substring(4);
                _logger.LogInformation("Received question for OpenAI: {Question}", question);

                try
                {
                    var response = await _openAIService.GetResponseToQuestionAsync(question);
                    _logger.LogInformation("Response from OpenAI: {Response}", response);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting response from OpenAI");
                    return $"I'm sorry, I can't answer that question right now. \n {ex.Message}";
                }
            }
            else
            {
                switch (message)
                {
                    case "ping":
                        _logger.LogInformation("Responding to ping request");
                        return "Pong!";
                    case "ip":
                        _logger.LogInformation("Responding to IP request");
                        return await _getPublicIPQuery.GetPublicIP();
                    default:
                        _logger.LogInformation("Responding with confused response");
                        return GetConfusedResponse();
                }
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
