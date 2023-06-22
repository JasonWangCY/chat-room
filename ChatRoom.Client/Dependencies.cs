using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using ChatRoom.Client.Services.Interfaces;
using ChatRoom.Client.Services;

namespace ChatRoom.Client;

public static class Dependencies
{
    public static void SetUpLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, theme: AnsiConsoleTheme.Code)
            .CreateLogger();
    }

    public static ServiceCollection RegisterServices(this ServiceCollection services)
    {
        services.AddLogging(x => x.AddSerilog())
            .AddSingleton(Log.Logger)
            .AddSingleton<IChatClient, ChatClient>();

        return services;
    }
}