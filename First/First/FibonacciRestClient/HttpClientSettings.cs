namespace First.FibonacciRestClient
{
    public class HttpClientSettings
    {
        public string Uri { get; set; }

        public static HttpClientSettings GetDefaultValue()
        {
            return new HttpClientSettings()
            {
                Uri = "http://localhost:5000/Fibonacci/calculate",
            };
        }
    }
}