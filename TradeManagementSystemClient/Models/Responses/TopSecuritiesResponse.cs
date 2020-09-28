using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class InstrumentType
    {
        [JsonProperty("activeStatus")]
        public string ActiveStatus { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class Country
    {
        [JsonProperty("activeStatus")]
        public string ActiveStatus { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("country")]
        public string MyCountry { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("currencySymbol")]
        public string CurrencySymbol { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    public class Exchange
    {
        [JsonProperty("activeStatus")]
        public string ActiveStatus { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("contactNumber1")]
        public string ContactNumber1 { get; set; }

        [JsonProperty("contactNumber2")]
        public string ContactNumber2 { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }
    }

    public class TopSecuritiesResponse : ITopSecuritiesResponse
    {
        [JsonProperty("securitySymbol")]
        public string SecuritySymbol { get; set; }

        [JsonProperty("securityName")]
        public string SecurityName { get; set; }

        [JsonProperty("totalTurnover")]
        public double TotalTurnover { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("totalTransaction")]
        public int TotalTransaction { get; set; }

        [JsonProperty("lastTradePrice")]
        public double LastTradePrice { get; set; }

        [JsonProperty("perChange")]
        public double PerChange { get; set; }

        [JsonProperty("prevClosePrice")]
        public double PrevClosePrice { get; set; }
    }
}
