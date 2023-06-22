using ChatRoom.Client.Services.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Client.Services;

public class ChatClient : IChatClient
{
    private string _userName = null!;

    public void Execute(int port, CancellationTokenSource cancellationTokenSrc)
    {
        var host = Dns.GetHostEntry("localhost");
        var ipAddress = host.AddressList.First();
        var ipEndPoint = new IPEndPoint(ipAddress, port);

        try
        {
            Console.WriteLine($"Connecting to the chat server at {ipEndPoint}");
            var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);

            Console.WriteLine("Connected to the chat server");

            new ReadThread(socket, this).Start(cancellationTokenSrc);
            new WriteThread(socket, this).Start(cancellationTokenSrc);
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