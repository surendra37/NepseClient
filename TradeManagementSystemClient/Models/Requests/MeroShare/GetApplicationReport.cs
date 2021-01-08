using System.Collections.Generic;

namespace TradeManagementSystemClient.Models.Requests.MeroShare
{
    public class FilterFieldParam
    {
        public string Key { get; set; }
        public string Alias { get; set; }
    }

    public class FilterDateParam
    {
        public string Key { get; set; }
        public string Condition { get; set; }
        public string Alias { get; set; }
        public string Value { get; set; }
    }

    public class GetApplicationReport
    {
        public List<FilterFieldParam> FilterFieldParams { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public string SearchRoleViewConstants { get; set; }
        public List<FilterDateParam> FilterDateParams { get; set; }

        /// <summary>
        /// SearchRoleViewConstants value: "VIEW", "VIEW_APPLICANT_FORM_COMPLETE"
        /// </summary>
        /// <param name="roleView"></param>
        public GetApplicationReport(string roleView)
        {
            Page = 1;
            Size = 200;
            SearchRoleViewConstants = roleView;
            FilterDateParams = new List<FilterDateParam>
            {
                new FilterDateParam
                {
                    Key = "appliedDate",
                    Condition = string.Empty,
                    Alias = string.Empty,
                    Value = string.Empty,
                },
                new FilterDateParam
                {
                    Key = "appliedDate",
                    Condition = string.Empty,
                    Alias = string.Empty,
                    Value = string.Empty,
                },
            };
            FilterFieldParams = new List<FilterFieldParam>
            {
                new FilterFieldParam
                {
                    Alias = "Scrip",
                    Key = "companyShare.companyIssue.companyISIN.script",
                },
                new FilterFieldParam
                {
                    Alias = "Company Name",
                    Key = "companyShare.companyIssue.companyISIN.company.name",
                },
            };
        }
    }
}
