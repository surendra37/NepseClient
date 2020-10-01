using System;

namespace NepseClient.Commons.Contracts
{
    public interface IMeroshareOwnDetail
    {
        string Address { get; set; }
        string Boid { get; set; }
        string ClientCode { get; set; }
        string Contact { get; set; }
        DateTime CreatedApproveDate { get; set; }
        string CustomerTypeCode { get; set; }
        string Demat { get; set; }
        string Email { get; set; }
        DateTime ExpiredDate { get; set; }
        string Gender { get; set; }
        int Id { get; set; }
        string ImagePath { get; set; }
        string MeroShareEmail { get; set; }
        string Name { get; set; }
        DateTime PasswordChangeDate { get; set; }
        DateTime PasswordExpiryDate { get; set; }
        string ProfileName { get; set; }
        bool RenderDashboard { get; set; }
        DateTime RenewedDate { get; set; }
        string Username { get; set; }
    }
}