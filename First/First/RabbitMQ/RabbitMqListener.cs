using System;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace First.RabbitMQ
{
    public class RabbitMqListener : IHostedService
    {
        private IModel _channel = null;
        private IConnection _connection = null;
        private readonly FibonacciCalculator _calculator;
        private readonly RabbitMqConnectionSettings _settings;

        public RabbitMqListener(FibonacciCalculator calculator)
        {
            _calculator = calculator;
            _settings = RabbitMqConnectionSettings.GetDefaultValue();
        }

        private void Run()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_settings.Uri)
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.Queue,
                durable: _settings.Durable,
                exclusive: _settings.Exclusive,
                autoDelete: _settings.AutoDelete,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceived;

            _channel.BasicConsume(queue: _settings.Queue,
                autoAck: _settings.AutoAck,
                consumer: consumer);
        }


        private void OnReceived(object channel, BasicDeliverEventArgs eventArgs) => Task.Run(() => OnReceivedAsync(eventArgs));
        private async Task OnReceivedAsync(BasicDeliverEventArgs eventArgs)
        {
            var result = ReadAndVerifyReceivedMessage(eventArgs);
            await _calculator.RunAsync(ParseParameterAndVerify(result.Prior), ParseParameterAndVerify(result.Current));
        }

        private FibonacciParameters ReadAndVerifyReceivedMessage(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            FibonacciParameters result = null;
            try
            {
                result = JsonSerializer.Deserialize<FibonacciParameters>(message);
                return result;
            }
            catch(Exception ex)
            {
                throw new SerializationException($"Error while FibonacciParameters deserialization: {message}", ex);
            }
        }

        private BigInteger ParseParameterAndVerify(string source)
        {
            if (BigInteger.TryParse(source, out BigInteger value))
            {
                if (value > 0)
                {
                    return value;
                }
            }

            throw new ArgumentException($" Parameter {source} is incorrect. Positive BigInteger value is expected.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Dispose();
            _connection.Dispose();
            return Task.CompletedTask;
        }
    }
}