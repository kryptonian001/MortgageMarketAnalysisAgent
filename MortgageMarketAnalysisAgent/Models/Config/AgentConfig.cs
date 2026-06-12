using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;

namespace MortgageMarketAnalysisAgent.Models.Config
{
    public class AgentConfig
    {
        public string ApplicationName { get; set; }
        public string GoogleConfigPath { get; set; }
        public string OpenAiKey { get; set; }
        public string NotificationEmail { get; set; }
        public string GoogleTokenPath { get; set; }
    }
}
