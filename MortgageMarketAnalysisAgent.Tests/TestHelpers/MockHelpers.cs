using Microsoft.Extensions.Logging;
using Moq;

namespace MortgageMarketAnalysisAgent.Tests.TestHelpers;

public static class MockHelpers
{
    public static Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    public static void VerifyLog<T>(
        this Mock<ILogger<T>> loggerMock,
        LogLevel logLevel,
        string expectedMessage,
        Times times)
    {
        loggerMock.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(expectedMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    public static void VerifyLogInformation<T>(
        this Mock<ILogger<T>> loggerMock,
        string expectedMessage)
    {
        loggerMock.VerifyLog(LogLevel.Information, expectedMessage, Times.AtLeastOnce());
    }

    public static void VerifyLogError<T>(
        this Mock<ILogger<T>> loggerMock,
        string expectedMessage)
    {
        loggerMock.VerifyLog(LogLevel.Error, expectedMessage, Times.AtLeastOnce());
    }

    public static void VerifyNoErrors<T>(
        this Mock<ILogger<T>> loggerMock)
    {
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
