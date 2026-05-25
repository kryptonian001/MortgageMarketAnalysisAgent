using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;

namespace MortgageMarketAnalysisAgent.Models.Config
{
    public class AgentConfig
    {
        public string GithubToken { get; set; }
        public string OpenAiKey { get; set; }

        public UserCredential GoogleCredential  { get; set; }
    }
}
