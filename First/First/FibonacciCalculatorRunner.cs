using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace First
{
    public class FibonacciCalculatorRunner : IHostedService
    {
        private readonly FibonacciCalculator _fibonacciCalculator;
        private readonly CommandLineArgs _commandLineArgs;

        public FibonacciCalculatorRunner(FibonacciCalculator fibonacciCalculator, CommandLineArgs commandLineArgs)
        {
            _fibonacciCalculator = fibonacciCalculator;
            _commandLineArgs = commandLineArgs;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var workerCount = GetWorkerCount();
            Task[] taskArray = new Task[workerCount];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew(() =>
                    _fibonacciCalculator.RunAsync(1, 1, cancellationToken), cancellationToken);
            }

            Task.WhenAll(taskArray);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private int GetWorkerCount()
        {
            if (_commandLineArgs.Args.Length == 0)
            {
                throw new ArgumentException("Numeric argument is expected");
            }

            if (int.TryParse(_commandLineArgs.Args[0], out int n))
            {
                if (n > 0)
                {
                    return n;
                }
            }

            throw new ArgumentException($"Positive, non-zero numeric argument is expected. But found {_commandLineArgs.Args[0]}");
        }
    }
}