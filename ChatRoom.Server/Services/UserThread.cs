using ChatRoom.Server.Services.Interfaces;
using Serilog;
using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Server.Services;

public class UserThread : IUserThread
{
    private readonly Socket _handler;
    private readonly ChatServer _server;

    private StreamWriter _writer = null!;

    public UserThread(
        Socket handler,
        ChatServer server)
    {
        _handler = handler;
        _server = server;
    }

    public void Start()
    {
        var threadDelegate = new ThreadStart(Run);
        var thread = new Thread(threadDelegate);
        thread.Start();
    }

    public async void Run()
    {
        try
        {
            await PrintUsersAsync();

            var buffer = new byte[1_024];
            var received = await _handler.ReceiveAsync(buffer, SocketFlags.None);
            var userName = Encoding.UTF8.GetString(buffer, 0, received);
            while (string.IsNullOrEmpty(userName))
            {

            }
            _server.AddUserName(userName);

            var serverMessage = $"New user connected: {userName}";
            Log.Debug(serverMessage);
            await _server.BroadcastAsync(serverMessage, this);

            var clientMessage = string.Empty;
            while (!string.Equals(clientMessage, "quit"))
            {
                var time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                buffer = new byte[1_024];
                received = await _handler.ReceiveAsync(buffer, SocketFlags.None);
                clientMessage = Encoding.UTF8.GetString(buffer, 0, received);
                serverMessage = $"{time} [{userName}]: {clientMessage}";

                Log.Debug(serverMessage);
                await _server.BroadcastAsync(serverMessage, this);
            }

            _server.RemoveUser(userName, this);
            _handler.Close();

            serverMessage = $"{userName} has quit";
            Log.Debug(serverMessage);
            await _server.BroadcastAsync(serverMessage, this);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in User Thread... {ex}");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _handler.SendAsync(messageBytes, SocketFlags.None);
    }

    private async Task PrintUsersAsync()
    {
        var message = string.Empty;
        if (_server.HasUsers())
        {
            message = $"Connected users: {string.Join(", ", _server.GetUserNames())}";
        }
        else
        {
            message = "No other users connected";
        }

        var messageBytes = Encoding.UTF8.GetBytes(message);
        await _handler.SendAsync(messageBytes, SocketFlags.None);
    }
}