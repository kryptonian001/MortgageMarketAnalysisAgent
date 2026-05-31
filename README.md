# Mortgage Market Analysis Agent

## Overview

The **Mortgage Market Analysis Agent** is an intelligent .NET 10.0 console application that automates household financial analysis with a specific focus on mortgage refinance readiness. The agent integrates with Google Sheets to read financial data, leverages OpenAI's GPT models to generate actionable insights, and delivers comprehensive analysis reports via email.

This autonomous agent helps households make informed decisions about mortgage refinancing by analyzing:
- Credit card utilization and debt management
- Cash flow patterns and upcoming payment obligations
- Credit profile readiness for refinancing
- Short-term and long-term loan portfolios
- Monthly bills and income streams
- Pay-cycle pressure and financial sustainability

## Key Features

- **Automated Financial Data Collection**: Reads household financial data from a structured Google Sheets spreadsheet
- **AI-Powered Analysis**: Uses OpenAI GPT-4.1-mini to analyze financial data and generate personalized recommendations
- **Multi-dimensional Financial Analysis**:
  - Credit card utilization optimization
  - Cash flow safety preservation
  - Mortgage refinance readiness assessment
  - Payment allocation recommendations
  - Pay-cycle pressure analysis
- **Email Notifications**: Automatically sends analysis reports via Gmail
- **Template-based Prompting**: Uses customizable prompt templates for consistent analysis
- **Data Quality Validation**: Identifies and reports data quality issues in source spreadsheets
- **Docker Support**: Containerized deployment for easy scheduling and automation

## Code Quality Report Card

| Category | Grade | Score |
|----------|-------|-------|
| **Architecture & Design** | A+ | 25/25 |
| **Code Quality** | A- | 18/20 |
| **Best Practices** | A | 19/20 |
| **Testing** | B+ | 17/20 |
| **Error Handling & Resilience** | A+ | 10/10 |
| **Documentation** | A | 5/5 |
| **Overall** | **A-** | **91/100** |

**Test Coverage:** 39.7% (353/889 lines) | **Tests:** 102/102 passing ✅

---

## Architecture

The application follows a clean architecture pattern with clear separation of concerns:

```
MortgageMarketAnalysisAgent/
├── Agents/                    # AI agent implementations
│   ├── Concretes/
│   │   └── MarketAnalysisAgent.cs
│   └── Interfaces/
│       └── IAgent.cs
├── Services/                  # Business logic services
│   ├── Concretes/
│   │   ├── MarketAnalysisService.cs
│   │   ├── HouseholdFinancialIntelligenceReportBuildingService.cs
│   │   ├── HouseholdFinancialPromptBuilder.cs
│   │   ├── GoogleDocumentService.cs
│   │   └── GoogleNotificationService.cs
│   └── Interfaces/
├── Models/                    # Data models
│   ├── Config/
│   │   └── AgentConfig.cs
│   └── Documents/
│       ├── HouseholdFinancialIntelligenceModel.cs
│       └── Components/        # Financial data components
├── Helpers/                   # Utility classes
├── Templates/                 # Prompt and report templates
│   ├── Prompts/
│   │   └── HiFi.pmt          # AI prompt template
│   └── Reports/
│       └── Analysis.rtl       # Report template
└── Program.cs                 # Application entry point
```

## Prerequisites

Before running the agent, ensure you have:

1. **.NET 10.0 SDK** installed
2. **Google Cloud Project** with the following APIs enabled:
   - Google Sheets API
   - Google Docs API
   - Gmail API
3. **OpenAI API Key** with access to GPT-4.1-mini
4. **Google OAuth 2.0 Credentials** (client_secret.json)
5. A structured **Google Sheets spreadsheet** with the following sheets:
   - Agent Dashboard
   - Income
   - Monthly Bills
   - Credit Cards
   - Credit Profiles
   - Loans
   - Short-Term Financing
   - Paycheck Cash Flow
   - Mortgage Refi Readiness

## Configuration

### 1. Google Cloud Setup

1. Create a Google Cloud Project at [console.cloud.google.com](https://console.cloud.google.com)
2. Enable the required APIs:
   - Google Sheets API
   - Google Docs API
   - Gmail API
3. Create OAuth 2.0 credentials:
   - Go to "APIs & Services" > "Credentials"
   - Create OAuth 2.0 Client ID (Desktop application)
   - Download the credentials as `client_secret.json`
4. Place `client_secret.json` in the project directory

### 2. Application Settings

Configure the agent using **User Secrets** (recommended for development) or **Environment Variables** (recommended for production). The `appsettings.json` file is intentionally kept empty for security best practices.

#### Configuration Properties

The agent requires the following configuration settings:

| Property | Description | Required |
|----------|-------------|----------|
| `ApplicationName` | Name of the application for Google API identification | Yes |
| `GoogleConfigPath` | Full path to the `client_secret.json` OAuth credentials file | Yes |
| `NotificationEmail` | Email address to receive analysis reports | Yes |
| `OpenAIKey` | OpenAI API key with access to GPT-4.1-mini | Yes |

#### Using User Secrets (Recommended for Development)

User Secrets provide secure storage for sensitive configuration during development:

```bash
# Initialize user secrets (if not already done)
dotnet user-secrets init --project MortgageMarketAnalysisAgent

# Set configuration values
dotnet user-secrets set "AgentConfig:ApplicationName" "Mortgage Analysis Agent" --project MortgageMarketAnalysisAgent
dotnet user-secrets set "AgentConfig:GoogleConfigPath" "C:\path\to\client_secret.json" --project MortgageMarketAnalysisAgent
dotnet user-secrets set "AgentConfig:NotificationEmail" "your-email@example.com" --project MortgageMarketAnalysisAgent
dotnet user-secrets set "AgentConfig:OpenAIKey" "sk-your-openai-api-key" --project MortgageMarketAnalysisAgent
```

**View your secrets:**
```bash
dotnet user-secrets list --project MortgageMarketAnalysisAgent
```

**Remove a secret:**
```bash
dotnet user-secrets remove "AgentConfig:OpenAIKey" --project MortgageMarketAnalysisAgent
```

**Clear all secrets:**
```bash
dotnet user-secrets clear --project MortgageMarketAnalysisAgent
```

#### Using Environment Variables (Recommended for Production)

Environment variables are ideal for production deployments and CI/CD pipelines:

**Windows (PowerShell):**
```powershell
$env:AgentConfig__ApplicationName = "Mortgage Analysis Agent"
$env:AgentConfig__GoogleConfigPath = "C:\path\to\client_secret.json"
$env:AgentConfig__NotificationEmail = "your-email@example.com"
$env:AgentConfig__OpenAIKey = "sk-your-openai-api-key"
```

**Windows (Command Prompt):**
```cmd
set AgentConfig__ApplicationName=Mortgage Analysis Agent
set AgentConfig__GoogleConfigPath=C:\path\to\client_secret.json
set AgentConfig__NotificationEmail=your-email@example.com
set AgentConfig__OpenAIKey=sk-your-openai-api-key
```

**Linux/Mac:**
```bash
export AgentConfig__ApplicationName="Mortgage Analysis Agent"
export AgentConfig__GoogleConfigPath="/path/to/client_secret.json"
export AgentConfig__NotificationEmail="your-email@example.com"
export AgentConfig__OpenAIKey="sk-your-openai-api-key"
```

**Docker Environment Variables:**
```bash
docker run -d \
  -v /path/to/client_secret.json:/app/client_secret.json \
  -e AgentConfig__ApplicationName="Mortgage Analysis Agent" \
  -e AgentConfig__GoogleConfigPath="/app/client_secret.json" \
  -e AgentConfig__NotificationEmail="your-email@example.com" \
  -e AgentConfig__OpenAIKey="sk-your-openai-api-key" \
  mortgage-analysis-agent
```

#### Using appsettings.json (Not Recommended)

While `appsettings.json` can be used for configuration, it is **strongly discouraged** for security reasons. If you must use it:

```json
{
  "AgentConfig": {
    "ApplicationName": "Mortgage Analysis Agent",
    "GoogleConfigPath": "path/to/client_secret.json",
    "NotificationEmail": "your-email@example.com",
    "OpenAIKey": "sk-your-openai-api-key"
  }
}
```

⚠️ **Warning:** Never commit `appsettings.json` with actual credentials to version control!

### 3. Google Sheets Setup

The agent expects a Google Sheets spreadsheet with specific structure. Update the spreadsheet ID in `HouseholdFinancialIntelligenceModel.cs`:

```csharp
public static readonly string SHEET_ID = "your-google-sheet-id";
```

Required sheets and their data ranges are defined in `HouseholdFinancialIntelligenceModel.ReportCells`.

## Installation

### Local Installation

1. Clone the repository:
```bash
git clone https://github.com/kryptonian001/MortgageMarketAnalysisAgent.git
cd MortgageMarketAnalysisAgent
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Configure the application (see Configuration section above)

4. Build the project:
```bash
dotnet build
```

### Docker Installation

1. Build the Docker image:
```bash
docker build -t mortgage-analysis-agent .
```

2. Run the container with configuration:
```bash
docker run -d \
  -v /path/to/client_secret.json:/app/client_secret.json \
  -e AgentConfig__GoogleConfigPath="/app/client_secret.json" \
  -e AgentConfig__NotificationEmail="your-email@example.com" \
  -e AgentConfig__OpenAIKey="sk-your-openai-api-key" \
  mortgage-analysis-agent
```

## Usage

### Running the Agent

**Local execution:**
```bash
dotnet run --project MortgageMarketAnalysisAgent
```

**Published executable:**
```bash
dotnet publish -c Release
cd bin/Release/net10.0/publish
dotnet MortgageMarketAnalysisAgent.dll
```

### Execution Flow

When executed, the agent performs the following steps:

1. **Authentication**: Authenticates with Google APIs using OAuth 2.0
2. **Data Collection**: Reads financial data from the configured Google Sheets spreadsheet
3. **Data Processing**: Builds a comprehensive financial intelligence report
4. **Prompt Generation**: Creates a detailed AI prompt with financial context
5. **AI Analysis**: Sends the prompt to OpenAI GPT-4.1-mini for analysis
6. **Report Delivery**: Emails the analysis report to the configured email address

Console output during execution:
```
Retrieving Household_Financial_Intelligence_Agent_Ready spreadsheet
Building market analysis prompt with Household_Financial_Intelligence_Agent_Ready information
Sending prompt to ChatGPT
Results:
[Analysis output...]
Sending to email: your-email@example.com
```

### Scheduling Automated Runs

**Windows (Task Scheduler):**
```powershell
$action = New-ScheduledTaskAction -Execute "dotnet" -Argument "path\to\MortgageMarketAnalysisAgent.dll"
$trigger = New-ScheduledTaskTrigger -Weekly -DaysOfWeek Sunday -At 9am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "MortgageAnalysisAgent"
```

**Linux/Mac (Cron):**
```bash
# Add to crontab (runs every Sunday at 9am)
0 9 * * 0 cd /path/to/app && dotnet MortgageMarketAnalysisAgent.dll
```

**Docker with Cron:**
```bash
# Schedule with a cron container
docker run -d \
  --name mortgage-agent-scheduler \
  -e CRON_SCHEDULE="0 9 * * 0" \
  mortgage-analysis-agent
```

## Analysis Capabilities

The agent provides comprehensive financial analysis including:

### Primary Goals
1. **Mortgage Refinance Readiness**: Evaluates credit profile and financial position for refinancing
2. **Cash-Flow Safety**: Ensures sufficient buffer for unexpected expenses
3. **Credit Card Optimization**: Recommends strategies to reduce utilization
4. **Risk Mitigation**: Identifies and avoids unnecessary financial risks
5. **Payment Allocation**: Suggests optimal payment strategies
6. **Pay-Cycle Analysis**: Analyzes upcoming payment pressure and sustainability

### Analysis Rules
- Maintains 25% cash buffer (recommends using max 75% of available cash)
- Prioritizes minimum payments first
- Does not recommend opening/closing credit accounts
- Explicitly marks uncertain or inferred data
- Provides practical, household-specific recommendations
- Validates data quality before making recommendations

## Dependencies

Key NuGet packages used:

- **Google.Apis.Sheets.v4** (1.74.0.4061) - Google Sheets integration
- **Google.Apis.Docs.v1** (1.74.0.4134) - Google Docs API
- **Google.Apis.Gmail.v1** (1.74.0.4148) - Email notifications
- **OpenAI** (2.10.0) - AI-powered analysis
- **Microsoft.Extensions.Hosting** (10.0.8) - Application hosting
- **Microsoft.Extensions.Configuration.UserSecrets** (10.0.8) - Secure configuration
- **Markdig** (1.2.0) - Markdown processing
- **System.CommandLine** (3.0.0-preview.3.26207.106) - CLI support

## Troubleshooting

### Common Issues

**Authentication Errors:**
- Ensure `client_secret.json` path is correct
- Verify Google Cloud APIs are enabled
- Check OAuth consent screen configuration
- Delete token-store directory and re-authenticate

**OpenAI API Errors:**
- Verify API key is valid and has credits
- Check model availability (GPT-4.1-mini)
- Monitor rate limits

**Google Sheets Errors:**
- Verify spreadsheet ID is correct
- Ensure sheet names match expected values
- Check data ranges in `HouseholdFinancialIntelligenceModel.ReportCells`
- Verify user has access to the spreadsheet

**Email Delivery Issues:**
- Confirm Gmail API is enabled
- Check email address configuration
- Verify OAuth scopes include `GmailService.Scope.GmailSend`

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Create a Pull Request

## License

This project is available for personal and educational use.

## Security Notes

⚠️ **Important Security Considerations:**

- Never commit `appsettings.json` with actual credentials
- Use User Secrets for development
- Use environment variables or secure vaults for production
- Rotate API keys regularly
- Keep `client_secret.json` secure and private
- Review OAuth token permissions periodically

## Support

For issues, questions, or contributions:
- GitHub Issues: [https://github.com/kryptonian001/MortgageMarketAnalysisAgent/issues](https://github.com/kryptonian001/MortgageMarketAnalysisAgent/issues)
- Repository: [https://github.com/kryptonian001/MortgageMarketAnalysisAgent](https://github.com/kryptonian001/MortgageMarketAnalysisAgent)

## Roadmap

Potential future enhancements:
- [ ] Support for multiple household profiles
- [ ] Web dashboard for visualizing analysis results
- [ ] Integration with additional financial data sources
- [ ] Customizable analysis rules and thresholds
- [ ] Historical trend analysis and tracking
- [ ] Multiple AI model support
- [ ] Enhanced data quality reporting
- [ ] Automated spreadsheet template generation
