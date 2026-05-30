using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Agents.Interfaces;
using MortgageMarketAnalysisAgent.Models.Config;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Agents.Concretes
{
    public class MarketAnalysisAgent : IAgent
    {
        private readonly ChatClient chatClient;


        public MarketAnalysisAgent(IOptions<AgentConfig> options)
        {
            ArgumentNullException.ThrowIfNull(options?.Value);

            var config = options.Value;
            chatClient = new ChatClient("gpt-4.1-mini", config.OpenAiKey);
        }


        public async Task<string> RunAnalysisAsync(string mortgageReadiness)
        {
            ChatCompletion completion = await chatClient.CompleteChatAsync(
            [
                new UserChatMessage(mortgageReadiness)
            ]);

            return completion.Content[0].Text;
        }
    }
}
