using Serilog;
using Microsoft.AspNetCore.SignalR;
using Serilog.Configuration;
using SimpleRDBMSRestfulAPI;

public static class SignalRLoggerConfigurationExtensions
{
    public static LoggerConfiguration SignalR(
        this LoggerSinkConfiguration loggerSinkConfiguration,
        IHubContext<NotificationHub> hubContext,
        IFormatProvider formatProvider = null)
    {
        return loggerSinkConfiguration.Sink(new SignalRSink(hubContext, formatProvider));
    }
}