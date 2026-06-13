using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Models.Tasks;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class GoogleDocumentService : IExternalDocumentService
    {
        private readonly SheetsService sheetsService;
        private readonly ILogger<GoogleDocumentService> _logger;

        public GoogleDocumentService(UserCredential credential, AgentConfig? config, ILogger<GoogleDocumentService> logger)
        {
            var init = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = config?.ApplicationName ?? ""
            };

            sheetsService = new SheetsService(init);

            _logger = logger;
        }

        public async Task<string> ReadDocument(string docId)
        {
            return "";
        }

        public async Task<IList<IList<object>>> ReadRangeAsync(string sheetId, string range)
        {
            _logger.LogInformation($"Pulling data form range: {range}");
            SpreadsheetsResource.ValuesResource.GetRequest request =
                 sheetsService.Spreadsheets.Values.Get(sheetId, range);

            ValueRange reposne = await request.ExecuteAsync();

            return reposne.Values ?? default(IList<IList<object>>);
        }

        public async Task WriteDocument(string docName, string content)
        {

        }

    }
}
