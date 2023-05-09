﻿using ChatNet.Data.Models.Settings;
using ChatNet.Utils.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatNet.Service.Processes
{
    public class StockBot
    {
        private readonly IModel _channel;
        private readonly HttpClient _httpClient;
        private readonly ServiceSettings _settings;

        public StockBot(IServiceProvider serviceProvider)
        {
            var settings = serviceProvider.GetRequiredService<ServiceSettings>();
            ArgumentNullException.ThrowIfNull(settings);

            _settings = settings;

            var factory = new ConnectionFactory
            {
                HostName = _settings.MessageBroker.Server,
                UserName = _settings.MessageBroker.User,
                Password = _settings.MessageBroker.Password
            };

            _httpClient = new HttpClient();
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(
                queue: MessageBroker.RequestQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public void ListenAndReply()
        {
            Console.WriteLine("Listening for new messages in broker");
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, args) => { await GetShareQuoteAsync(sender, args); };
            _channel.BasicConsume(
                queue: MessageBroker.RequestQueue,
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
        private async Task GetShareQuoteAsync(object? sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine($"New message in broker received at: {DateTime.Now:dd MMM yyyy hh:mm:ss}");
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentException.ThrowIfNullOrEmpty(_settings.StockApiUrl);
            
            var botResponse = string.Empty;

            try
            {
                var ticker = GetTickerSymbol(args.Body.ToArray());
                var url = _settings.StockApiUrl.Replace("[ticker]", ticker); //$"https://stooq.com/q/l/?s={ticker}&f=sd2t2ohlcv&h&e=csv";
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
                    botResponse = $"{ticker.ToUpper()} quote is ${quote} per share";
                else
                    throw new InvalidDataException("Can't parse quote value");
            }
            catch (Exception ex)
            {
                botResponse = $"StockBot exception: {ex.Message}";
                throw;
            }
            finally
            {
                //TODO: Send botResponse via rabbitmq
            }
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
