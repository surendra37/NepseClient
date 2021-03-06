﻿namespace TradeManagementSystemClient.Models.Responses
{
    public class ResponseBase<T>
    {
        public string Status { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class ResponseBase 
    {
        public string Status { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
    }
}
