using Microsoft.AspNetCore.Mvc;
using Second.Models;

namespace Second.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FibonacciController : ControllerBase
    {
        private readonly CalculateFibonacciCommand _command;

        public FibonacciController(CalculateFibonacciCommand command)
        {
            _command = command;
        }

        [HttpGet]
        [Route("calculate")]
        public string Calculate([FromQuery] FibonacciParameters queryParameters)
        {
            var res = _command.CalculateAndSendNextMessage(queryParameters.Prior, queryParameters.Current);

            return res.ToString();
        }
    }
}