using System;
using System.Text;
using RabbitMQ.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Second.RabbitMQ
{
    public class RabbitMqService : IDisposable
    {
        private readonly IModel channel = null;
        private readonly IConnection connection = null;
        private readonly RabbitMqConnectionSettings _settings;

        public RabbitMqService()
        {
            _settings = RabbitMqConnectionSettings.GetDefaultValue();
            var factory = new ConnectionFactory() { Uri = new Uri(_settings.Uri) };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            channel.QueueDeclare(queue: _settings.Queue,
                durable: _settings.Durable,
                exclusive: _settings.Exclusive,
                autoDelete: _settings.AutoDelete,
                arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                routingKey: _settings.Queue,
                basicProperties: null,
                body: body);

            Console.WriteLine($"Published: {message}");
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }
    }
}