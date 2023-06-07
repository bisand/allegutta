using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AlleGutta.Yahoo;

public static class YahooFinance
{
    public const string HISTQUOTES2_SCRAPE_URL = "https://finance.yahoo.com/quote/%5EGSPC/options?guccounter=1";
    public const string HISTQUOTES2_CRUMB_URL = "https://query1.finance.yahoo.com/v1/test/getcrumb";
    public const string HISTQUOTES2_COOKIE_NAMESPACE = "yahoo";
    public const string HISTQUOTES2_COOKIE_AGREE = "agree";
}
