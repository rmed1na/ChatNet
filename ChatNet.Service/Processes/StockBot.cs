using ChatNet.Data.Models.Constants;
using ChatNet.Data.Models.Settings;
using ChatNet.Utils.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatNet.Service.Processes
{
    /// <summary>
    /// Background service that listens for new stock quote requests and return back the current stock quote on the market
    /// </summary>
    public class StockBot
    {
        private readonly IModel _channel;
        private readonly HttpClient _httpClient;
        private readonly ServiceSettings _settings;

        public StockBot(ServiceSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            _settings = settings;
            _httpClient = new HttpClient();

            var factory = new ConnectionFactory
            {
                HostName = _settings.MessageBroker.Server,
                UserName = _settings.MessageBroker.User,
                Password = _settings.MessageBroker.Password
            };            
            
            var connection = factory.CreateConnection();

            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: MessageBrokerParams.REQUEST_QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.QueueDeclare(
                queue: MessageBrokerParams.RESPONSE_QUEUE_NAME,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        /// <summary>
        /// Sets up the bot to listen and reply for stock quote requests
        /// </summary>
        public void ListenAndReply()
        {
            Console.WriteLine("Listening for new messages in broker");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) => 
            { 
                var result = await GetStockQuoteAsync(sender, args);
                Reply(result);
            };

            _channel.BasicConsume(
                queue:  MessageBrokerParams.REQUEST_QUEUE_NAME,
                autoAck: true,
                consumer: consumer);
        }

        /// <summary>
        /// Gets the stock price requested on the command and replies back
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private async Task<string> GetStockQuoteAsync(object? sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine($"New message in broker received at: {DateTime.Now:dd MMM yyyy hh:mm:ss} on [{MessageBrokerParams.REQUEST_QUEUE_NAME}]");
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentException.ThrowIfNullOrEmpty(_settings.StockApiUrl);

            try
            {
                var ticker = GetTickerSymbol(args.Body.ToArray());
                var url = _settings.StockApiUrl.Replace("[ticker]", ticker);
                var apiResponse = await _httpClient.GetAsync(url);

                apiResponse.EnsureSuccessStatusCode();

                var data = await apiResponse.Content.ReadAsStringAsync();
                var lines = data.Split('\n');

                if (lines.Length <= 0)
                    throw new InvalidDataException("File doesn't have any rows");

                var values = lines[1].Split(',');
                if (values.Length <= 0)
                    throw new InvalidDataException("Row doesn't have any data");

                if (decimal.TryParse(values[6], out decimal quote))
                    return $"{ticker.ToUpper()} quote is ${quote} per share";
                else
                    throw new InvalidDataException("Can't parse quote value");
            }
            catch (Exception ex)
            {
                return $"StockBot exception: {ex.Message}";
                throw;
            }
        }

        /// <summary>
        /// Gives back the quote response in the response queue
        /// </summary>
        /// <param name="response">The current stock quote on the market at close time</param>
        private void Reply(string response)
        {
            Console.WriteLine($"Sending back result ({response}) on [{MessageBrokerParams.RESPONSE_QUEUE_NAME}]");
            var bytes = Encoding.UTF8.GetBytes(response);

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: MessageBrokerParams.RESPONSE_QUEUE_NAME,
                basicProperties: null,
                body: bytes);
        }

        /// <summary>
        /// Processes the message from the broker and gives back the ticker symbol of the stock
        /// </summary>
        /// <param name="bytes">Message bytes array</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When the command is empty or null</exception>
        /// <exception cref="InvalidDataException">When the command is invalid</exception>
        private static string GetTickerSymbol(byte[] bytes)
        {
            var command = Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrEmpty(command))
                throw new InvalidOperationException("Missing command");

            if (!command.IsValidCommand())
                throw new InvalidDataException($"The command '{command}' is not valid");

            return command.Split('=')[1];
        }
    }
}
