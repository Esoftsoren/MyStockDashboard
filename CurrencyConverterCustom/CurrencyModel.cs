using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CurrencyConverterCustom
{
    public class CurrencyModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("terms")]
        public string Terms { get; set; }
        
        [JsonPropertyName("privacy")]
        public string Privacy { get; set; }
        
        // Timestamp is a UNIX timestamp (in seconds)
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        
        [JsonPropertyName("source")]
        public string Source { get; set; }
        
        // exchangerate.host returns the rates in a property called "quotes"
        [JsonPropertyName("quotes")]
        public Dictionary<string, double> Quotes { get; set; }
        
        // You may add an error property if needed
        [JsonPropertyName("error")]
        public ApiError Error { get; set; }
    }
    
}