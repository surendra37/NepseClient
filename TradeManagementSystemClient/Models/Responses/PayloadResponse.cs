using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class PayloadResponse<T>
    {
        [JsonProperty("data")]
        public T[] Data { get; set; }
    }
}