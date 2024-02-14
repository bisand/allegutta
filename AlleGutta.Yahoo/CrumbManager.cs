using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace AlleGutta.Yahoo
{
    public static partial class CrumbManager
    {
        private static string _crumb = "";
        private static string _cookie = "";
        private static readonly Regex _crumbCookieRegex = CrumbCookieRegex();

        private static readonly CookieContainer _cookieContainer = new CookieContainer();

        public static ILogger? Logger { get; internal set; }

        private static async Task SetCookieAsync(string? proxyURL)
        {
            using HttpClient client = GetHttpClient(proxyURL);
            using var request1 = new HttpRequestMessage(HttpMethod.Get, YahooFinance.HISTQUOTES2_SCRAPE_URL);

            FillHeaders(request1, "https://www.yahoo.com/");

            using var response1 = await client.SendAsync(request1);
            response1.EnsureSuccessStatusCode();
            var responseBody = await response1.Content.ReadAsStringAsync();

            // Do something with the response body
            foreach (var header in response1.Headers)
            {
                if (header.Key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var cookieField in header.Value)
                    {
                        foreach (var cookieValue in cookieField.Split(';'))
                        {
                            if (_crumbCookieRegex.IsMatch(cookieValue))
                            {
                                _cookie = cookieValue;
                                Logger?.LogDebug($"Set cookie from http request: {_cookie}");
                                return;
                            }
                        }
                    }
                }
            }

            // If cookie is not set, we should consent to activate cookie
            var isReader = new StreamReader(response1.Content.ReadAsStream());
            var patternPostForm = new Regex("<form method=\"post\" class=\"consent-form\"");
            var patternInput = new Regex("(<input type=\"hidden\" name=\")(.*?)(\" value=\")(.*?)(\">)");
            var datas = new Dictionary<string, string>();
            var postFind = false;
            // Read source to get params data for post request
            string line;
            while ((line = await isReader.ReadLineAsync()) != null)
            {
                if (patternPostForm.IsMatch(line))
                {
                    postFind = true;
                }

                if (postFind)
                {
                    var matcher = patternInput.Match(line);
                    if (matcher.Success)
                    {
                        var name = matcher.Groups[2].Value;
                        var value = matcher.Groups[4].Value;
                        datas.Add(name, WebUtility.UrlDecode(value));
                    }
                }
            }
            // If params are not empty, send the post request
            if (datas.Count > 0)
            {
                datas.TryAdd("namespace", YahooFinance.HISTQUOTES2_COOKIE_NAMESPACE);
                datas.TryAdd("agree", YahooFinance.HISTQUOTES2_COOKIE_AGREE);
                datas.TryAdd("originalDoneUrl", YahooFinance.HISTQUOTES2_SCRAPE_URL);

                using var request2 = new HttpRequestMessage(HttpMethod.Post, response1?.RequestMessage?.RequestUri);
                FillHeaders(request2, response1?.RequestMessage?.RequestUri?.ToString() ?? string.Empty);
                if (response1?.RequestMessage?.RequestUri != null)
                {
                    request2.Headers.Referrer = response1.RequestMessage.RequestUri;
                }
                request2.Headers.Host = response1?.RequestMessage?.RequestUri?.Host;
                request2.Headers.Add("Origin", new Uri(response1?.RequestMessage?.RequestUri?.GetLeftPart(UriPartial.Authority) ?? string.Empty).ToString());
                request2.Content = new FormUrlEncodedContent(datas);
                using var response2 = await client.SendAsync(request2);
                // response.EnsureSuccessStatusCode();
                responseBody = await response2.Content.ReadAsStringAsync();
                Logger?.LogDebug($"Set cookie from http request: {_cookie}");
                return;
            }

            Logger?.LogError("Failed to set cookie from http request. Historical quote requests will most likely fail.");
        }

        private static async Task SetCrumbAsync(string? proxyURL)
        {
            using HttpClient client = GetHttpClient(proxyURL);
            using var request1 = new HttpRequestMessage(HttpMethod.Get, YahooFinance.HISTQUOTES2_CRUMB_URL);

            FillHeaders(request1, YahooFinance.HISTQUOTES2_SCRAPE_URL);

            using var response1 = await client.SendAsync(request1);
            response1.EnsureSuccessStatusCode();
            var crumbResult = await response1.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(crumbResult))
            {
                _crumb = crumbResult.Trim();
                Logger?.LogDebug($"Set crumb from http request: {_crumb}");
            }
            else
            {
                Logger?.LogError("Failed to set crumb from http request. Historical quote requests will most likely fail.");
            }
        }

        public static void FillHeaders(HttpRequestMessage request, string referer)
        {
            request.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:109.0) Gecko/20100101 Firefox/113.0");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Referer", referer);
            request.Headers.Add("Connection", "keep-alive");
        }

        public static HttpClient GetHttpClient(string? proxyURL)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            WebProxy? webProxy = string.IsNullOrEmpty(proxyURL) ? null : new WebProxy(proxyURL);

            var socketHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromHours(12),
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = _cookieContainer,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                MaxConnectionsPerServer = 1,
                UseProxy = !string.IsNullOrEmpty(proxyURL),
                Proxy = webProxy,
            };

            var pollyHandler = new PolicyHttpMessageHandler(retryPolicy)
            {
                InnerHandler = socketHandler,
            };

            var httpClient = new HttpClient(pollyHandler);
            return httpClient;
        }

        public static async Task RefreshAsync(string? proxyURL)
        {
            await SetCookieAsync(proxyURL);
            await SetCrumbAsync(proxyURL);
        }

        public static async Task<string> GetCrumbAsync(string? proxyURL)
        {
            if (string.IsNullOrEmpty(_crumb))
            {
                await RefreshAsync(proxyURL);
            }
            return _crumb;
        }

        public static async Task<string> GetCookieAsync(string? proxyURL)
        {
            if (string.IsNullOrEmpty(_cookie))
            {
                await RefreshAsync(proxyURL);
            }
            return _cookie;
        }

        [GeneratedRegex("B=.*")]
        private static partial Regex CrumbCookieRegex();
    }
}