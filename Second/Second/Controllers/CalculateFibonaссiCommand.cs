using System;
using System.Numerics;
using Second.RabbitMQ;
using Second.Models;

namespace Second.Controllers
{
    public class CalculateFibonacciCommand
    {
        private readonly RabbitMqService _mqService;

        public CalculateFibonacciCommand(RabbitMqService mqService)
        {
            _mqService = mqService;
        }

        public BigInteger CalculateAndSendNextMessage(string prior, string current)
        {
            var result = ParseParameterAndVerify(prior) + ParseParameterAndVerify(current);
            _mqService.SendMessage(new FibonacciParameters { Prior = current, Current = result.ToString() });

            return result;
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
    }
}