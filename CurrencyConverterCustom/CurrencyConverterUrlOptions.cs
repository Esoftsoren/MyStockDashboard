using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurrencyConverterCustom
{
    public class CurrencyConverterUrlOptions
    {
        public CurrencyConverterUrlOptions(string url) => this.Url = url;

        // Optionally, you can restrict the response to specific currency codes.
        public List<string> Symbols { get; set; } = new List<string>();

        // For exchangerate.host, you can set the base (or “source”) currency.
        // (By default the API uses USD if not specified.)
        public string BaseSymbol { get; set; } = "USD";

        // The API access key.
        public string AccessKey { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Url);
            sb.Append("/live");
            sb.Append("?access_key=" + this.AccessKey);
            // Optionally, add the symbols parameter if any symbols are specified.
            if (this.Symbols.Any())
            {
                sb.Append("&currencies=" + string.Join(",", this.Symbols));
            }
            return sb.ToString();
        }
    }
}