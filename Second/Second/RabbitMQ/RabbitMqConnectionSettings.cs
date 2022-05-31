namespace Second.RabbitMQ
{
    public sealed class RabbitMqConnectionSettings
    {
        public string Uri { get; set; }

        public string Queue { get; set; }

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        // In real app use config file + IConfiguration
        public static RabbitMqConnectionSettings GetDefaultValue()
        {
            return new RabbitMqConnectionSettings()
            {
                Uri = "correct-url",
                Queue = "Fibonacci",
                Durable = true,
                Exclusive = false,
                AutoDelete = false
            };
        }
    }
}