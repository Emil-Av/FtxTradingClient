﻿namespace FTXTradingClient.FTX
{
    public class Client
    {
        public string ApiKey { get; }

        public string ApiSecret { get; }

        public Client()
        {
            ApiKey = "";
            ApiSecret = "";
        }

        public Client(string apiKey, string apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
        }

    }
}