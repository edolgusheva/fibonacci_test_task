using System;
using System.Collections.Specialized;
using System.Text;

namespace First.FibonacciRestClient
{
    public static class FibonacciClientUrlBuilder
    {
        public static string BuildUrl(string baseUrl, NameValueCollection queryParams)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(baseUrl))
            {
                sb.Append(baseUrl);
                sb.Append('/');
            }

            sb.Append(BuildQueryString(queryParams));

            return sb.ToString();
        }

        private static string BuildQueryString(NameValueCollection query)
        {
            if (query == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (string key in query.Keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                var values = query.GetValues(key);
                if (values == null)
                {
                    continue;
                }

                foreach (string value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
                }
            }

            return sb.ToString();
        }
    }
}