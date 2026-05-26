using System;
using System.Collections.Generic;
using System.Text;

namespace MortgageMarketAnalysisAgent.Services.Interfaces
{
    public interface INotify
    {
        Task SendEmailNotificationAsync(string emailAddress, string subject, string message);
    }
}
