using ChatRoom.Client.Services.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Client.Services;

public class ChatClient : IChatClient
{
    private string _userName = null!;
    internal volatile bool _isConnected = false;

    public void Execute(int port, CancellationTokenSource cancellationTokenSrc)
    {
        var host = Dns.GetHostEntry("localhost");
        var ipAddress = host.AddressList.First();
        var ipEndPoint = new IPEndPoint(ipAddress, port);

        // TODO: Use async for the threads and socket connections
        // TODO: Use interfaces
        try
        {
            Console.WriteLine($"Connecting to the chat server at {ipEndPoint}");
            var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);

            Console.WriteLine("Connected to the chat server");

            new WriteThread(socket, this).Start(cancellationTokenSrc);
            new ReadThread(socket, this).Start(cancellationTokenSrc);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Server connection issues: {ex}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }

    public void SetUserName(string userName)
    {
        _userName = userName;
    }

    public string GetUserName()
    {
        return _userName;
    }
}