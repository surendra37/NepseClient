using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
using System;

namespace TradeManagementSystemClient.Models.Responses
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MeroshareOwnDetailResponse : IMeroshareOwnDetail
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("boid")]
        public string Boid { get; set; }

        [JsonProperty("clientCode")]
        public string ClientCode { get; set; }

        [JsonProperty("contact")]
        public string Contact { get; set; }

        [JsonProperty("createdApproveDate")]
        public DateTime CreatedApproveDate { get; set; }

        [JsonProperty("customerTypeCode")]
        public string CustomerTypeCode { get; set; }

        [JsonProperty("demat")]
        public string Demat { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("expiredDate")]
        public DateTime ExpiredDate { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("meroShareEmail")]
        public string MeroShareEmail { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("passwordChangeDate")]
        public DateTime PasswordChangeDate { get; set; }

        [JsonProperty("passwordExpiryDate")]
        public DateTime PasswordExpiryDate { get; set; }

        [JsonProperty("profileName")]
        public string ProfileName { get; set; }

        [JsonProperty("renderDashboard")]
        public bool RenderDashboard { get; set; }

        [JsonProperty("renewedDate")]
        public DateTime RenewedDate { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}