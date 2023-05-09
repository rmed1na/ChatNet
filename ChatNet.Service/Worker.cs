using ChatNet.Service.Processes;

namespace ChatNet.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stockBot = new StockBot(_serviceProvider);
            stockBot.ListenAndReply();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000 * 60 * 5, stoppingToken); // cycle run every 5 minutes to keep alive the executable
            }
        }
    }
}