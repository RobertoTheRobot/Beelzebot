using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Beelzebot.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get request...");
            // do something
            return Ok();
        }

    }
}
