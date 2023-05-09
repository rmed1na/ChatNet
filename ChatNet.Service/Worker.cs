using ChatNet.Data.Models.Settings;
using ChatNet.Service.Processes;
using Microsoft.Extensions.Options;

namespace ChatNet.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceSettings _settings;

        public Worker(ILogger<Worker> logger, IOptions<ServiceSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stockBot = new StockBot(_settings);
            stockBot.ListenAndReply();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000 * 60 * 5, stoppingToken); // cycle run every 5 minutes to keep alive the executable
            }
        }
    }
}