using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using ChatRoom.Server.Services.Interfaces;
using ChatRoom.Server.Services;

namespace ChatRoom.Server;

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
            .AddSingleton<IChatServer, ChatServer>();

        return services;
    }
}