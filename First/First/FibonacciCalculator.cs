using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using First.FibonacciRestClient;

namespace First
{
    public class FibonacciCalculator
    {
        private readonly FibonacciHttpClient _fibonacciHttpClient;

        public FibonacciCalculator(FibonacciHttpClient fibonacciHttpClient)
        {
            _fibonacciHttpClient = fibonacciHttpClient;
        }

        public async Task RunAsync(BigInteger prior, BigInteger current, CancellationToken cancellationToken = default)
        {
            var result = CalculateFibonacci(prior, current);
            var r = await CalculateNextFibonacciUsingHttp(current, result, cancellationToken);
            Console.WriteLine(r);
        }

        private BigInteger CalculateFibonacci(BigInteger prior, BigInteger current)
        {
            return prior + current;
        }

        private async Task<string> CalculateNextFibonacciUsingHttp(BigInteger prior, BigInteger current, CancellationToken cancellationToken)
        {
            return await _fibonacciHttpClient.Get(prior, current, cancellationToken);
        }
    }
}