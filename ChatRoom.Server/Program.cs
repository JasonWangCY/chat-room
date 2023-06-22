﻿using ChatRoom.Server;
using ChatRoom.Server.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

Dependencies.SetUpLogger();
if (args.Length != 1)
{
    Log.Error("Syntax: dotnet ChatServer.dll <port-number>");
    Environment.Exit(1);
}

if (!int.TryParse(args.First(), out var portNum))
{
    Log.Error("The argument given is not a number.");
    Environment.Exit(1);
}

var services = new ServiceCollection();
services.RegisterServices();

var sp = services.BuildServiceProvider();
var chatServer = sp.GetRequiredService<IChatServer>();

var cancellationTokenSrc = new CancellationTokenSource();
chatServer.Execute(portNum, cancellationTokenSrc).Wait();