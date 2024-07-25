namespace Beelzebot.webapi.Queries
{
    public interface IGetPublicIPQuery
    {
        Task<string> GetPublicIP();
    }
    public class GetPublicIPQuery : IGetPublicIPQuery
    {
        private readonly ILogger<GetPublicIPQuery> _logger;
        private readonly HttpClient _httpClient;

        public GetPublicIPQuery(ILogger<GetPublicIPQuery> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }
        

        public async Task<string> GetPublicIP()
        {
            string url = "https://ifconfig.me/ip";
            _logger.LogInformation("Getting public IP address from {Url}", url);

            try
            {
                string response = await _httpClient.GetStringAsync(url);
                _logger.LogInformation("Public IP address: {PublicIP}", response);
                return response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error getting public IP address");
                return "I'm sorry, I couldn't get the public IP address.";
            }
            
        }


    }
}
