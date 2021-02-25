using System;

namespace NepseClient.Libraries.MeroShare.Models.Responses
{
    public class MeroshareOwnDetailResponse
    {
        public string Address { get; set; }
        public string Boid { get; set; }
        public string ClientCode { get; set; }
        public string Contact { get; set; }
        public DateTime CreatedApproveDate { get; set; }
        public string CustomerTypeCode { get; set; }
        public string Demat { get; set; }
        public string Email { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Gender { get; set; }
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string MeroShareEmail { get; set; }
        public string Name { get; set; }
        public DateTime PasswordChangeDate { get; set; }
        public DateTime PasswordExpiryDate { get; set; }
        public string ProfileName { get; set; }
        public bool RenderDashboard { get; set; }
        public DateTime RenewedDate { get; set; }
        public string Username { get; set; }
    }
}