using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using Markdig;
using Microsoft.Extensions.Options;
using MortgageMarketAnalysisAgent.Models.Config;
using MortgageMarketAnalysisAgent.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MortgageMarketAnalysisAgent.Services.Concretes
{
    public class GoogleNotificationService : INotify
    {
        private readonly GmailService gmailService;

        static MarkdownPipeline pipeline;

        public GoogleNotificationService(UserCredential credential, AgentConfig? config)
        {
            var init = new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = config.ApplicationName ?? ""
            };

            gmailService = new GmailService(init);

            pipeline = new MarkdownPipelineBuilder()
                            .UseAdvancedExtensions()
                            .Build();
        }

        public async Task SendEmailNotificationAsync(string emailAddress, string subject, string body)
        {
            var bodyHtml = BuildHtmlMessage(body);
            bodyHtml = StyleMarkdownHtml(bodyHtml);

            string rawMessage = BuildRawEmailMessage(emailAddress, subject, bodyHtml);

            var message = new Message
            {
                Raw = Base64UrlEncode(rawMessage)
            };

            await gmailService.Users
                              .Messages
                              .Send(message, "me")
                              .ExecuteAsync();
        }

        private static string BuildHtmlMessage(string body)
        {
            string bodyHtml = Markdown.ToHtml(body, pipeline);

            string finalHtml = WrapHtmlReport(bodyHtml);

            return finalHtml;
        }

        private static string WrapHtmlReport(string bodyHtml)
        {
            return $$"""
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="utf-8">
                    <style>
                        body {
                            font-family: Arial, sans-serif;
                            line-height: 1.5;
                            color: #222;
                            margin: 0;
                            padding: 24px;
                            background-color: #f6f6f6;
                        }

                        .report-container {
                            max-width: 950px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 28px;
                            border-radius: 8px;
                        }

                        table {
                            border-collapse: collapse;
                            width: 100%;
                            margin: 16px 0 24px 0;
                        }

                        th, td {
                            border: 1px solid #d9d9d9;
                            padding: 9px 10px;
                            text-align: left;
                            vertical-align: top;
                        }

                        th {
                            background-color: #f2f2f2;
                            font-weight: 700;
                        }

                        h1 {
                            margin-top: 0;
                            font-size: 26px;
                        }

                        h2 {
                            margin-top: 32px;
                            border-bottom: 1px solid #ddd;
                            padding-bottom: 6px;
                            font-size: 22px;
                        }

                        h3 {
                            margin-top: 24px;
                            font-size: 18px;
                        }

                        ul {
                            padding-left: 24px;
                        }

                        li {
                            margin-bottom: 6px;
                        }
                    </style>
                </head>
                <body>
                    <div class="report-container">
                        {{bodyHtml}}
                    </div>
                </body>
                </html>
                """;
        }

        private static string StyleMarkdownHtml(string html)
        {
            return html
                .Replace("<table>", "<table style=\"border-collapse: collapse; width: 100%; margin: 16px 0;\">")
                .Replace("<th>", "<th style=\"border: 1px solid #ddd; padding: 8px; background-color: #f3f3f3; text-align: left;\">")
                .Replace("<td>", "<td style=\"border: 1px solid #ddd; padding: 8px; text-align: left;\">")
                .Replace("<h2>", "<h2 style=\"margin-top: 28px; color: #111; border-bottom: 1px solid #ddd; padding-bottom: 4px;\">")
                .Replace("<h3>", "<h3 style=\"margin-top: 20px; color: #222;\">")
                .Replace("<code>", "<code style=\"background-color: #f3f3f3; padding: 2px 4px; border-radius: 4px;\">");
        }

        private static string BuildRawEmailMessage(
        string to,
        string subject,
        string body)
        {
            return
                $"To: {to}\r\n" +
                $"Subject: {subject}\r\n" +
                "MIME-Version: 1.0\r\n" +
                "Content-Type: text/html; charset=utf-8\r\n" +
                "\r\n" +
                body;
        }

        private static string Base64UrlEncode(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
