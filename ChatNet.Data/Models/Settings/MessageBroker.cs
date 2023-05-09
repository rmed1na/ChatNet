﻿namespace ChatNet.Data.Models.Settings
{
    public class MessageBroker
    {
        public string? Server { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public static string RequestQueue => "ChatNet.Stock.Req";
        public static string ResponseQueue => "ChatNet.Stock.Res";
    }
}