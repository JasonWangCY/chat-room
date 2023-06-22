using ChatRoom.Server.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Server.Services;

public class ChatServer : IChatServer
{
    private readonly ILogger<ChatServer> _logger;

    private readonly HashSet<string> _userNames = new();
    private readonly HashSet<UserThread> _userThreads = new();

    public ChatServer(
        ILogger<ChatServer> logger)
    {
        _logger = logger;
    }

    public async Task Execute(int port, CancellationTokenSource cancellationTokenSrc)
    {
        var host = Dns.GetHostEntry("localhost");
        var ipAddress = host.AddressList.First();
        var ipEndPoint = new IPEndPoint(ipAddress, port);

        try
        {
            var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipEndPoint);
            listener.Listen(100);

            _logger.LogInformation($"Chat Server is listening on endpoint {ipEndPoint}");

            while (!cancellationTokenSrc.IsCancellationRequested)
            {
                var handler = await listener.AcceptAsync();
                _logger.LogInformation("New user connected");

                var newUser = new UserThread(handler, this);
                _userThreads.Add(newUser);
                newUser.Start();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in the server...");
        }
    }

    public async Task BroadcastAsync(string message, UserThread author)
    {
        var broadcastTasks = new List<Task>();

        foreach (var user in _userThreads)
        {
            if (user == author) continue;

            var broadcastTask = user.SendMessageAsync(message);
            broadcastTasks.Add(broadcastTask);
        }

        await Task.WhenAll(broadcastTasks);
    }

    public void AddUserName(string userName)
    {
        _userNames.Add(userName);
    }

    public void RemoveUser(string userName, UserThread user)
    {
        var removed = _userNames.Remove(userName);
        if (removed)
        {
            _userThreads.Remove(user);
            _logger.LogInformation($"The user {userName} has left");
        }
    }

    public HashSet<string> GetUserNames()
    {
        return _userNames;
    }

    public bool HasUsers()
    {
        return _userNames.Any();
    }
}
