using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealTimeCharts_Server.DataStorage;
using RealTimeCharts_Server.HubConfig;
using RealTimeCharts_Server.Services;
using RealTimeCharts_Server.TimerFeatures;

namespace RealTimeCharts_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ILogger<ChartController> _logger;
        private IHubContext<ChartHub> _hub;
        private readonly IBackgroundWorkerServiceOptions _options;

        public ChartController(ILogger<ChartController> logger,   IHubContext<ChartHub> hub, IBackgroundWorkerServiceOptions options)
        {
            _logger = logger;
            _hub = hub;
            _options = options;
        }

        public IActionResult Get()
        { 

            _logger.LogInformation("Get operation");
            // var timerManager = new TimerManager(() => _hub.Clients.All.SendAsync("transferchartdata", DataManager.GetData()));
            return Ok(new { Message = "Request Completed" }); 
        }

        [HttpPost("SetSender")]
        public IActionResult SetSender(bool sendMessages)
        {
            _logger.LogInformation($"SetSender = {sendMessages}");

            this._options.SendMessages = true;
            return Ok(new { Message = $"Send messages = {sendMessages}" });
        }

        [HttpGet("GetSender")]
        public IActionResult GetSender()
        {
            _logger.LogInformation($"GetSender()...");
            return Ok(new { Message = "Request Completed GetSender()" });

        }
    }
}