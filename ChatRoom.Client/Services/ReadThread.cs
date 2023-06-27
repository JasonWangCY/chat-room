using Microsoft.AspNetCore.DataProtection;
using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Client.Services;

public class ReadThread
{
    private readonly Socket _socket;
    private readonly ChatClient _chatClient;

    public ReadThread(Socket socket, ChatClient chatClient)
    {
        _socket = socket;
        _chatClient = chatClient;
    }

    public void Start(CancellationTokenSource cancellationTokenSrc)
    {
        var threadDelegate = new ThreadStart(() => Run(cancellationTokenSrc));
        var thread = new Thread(threadDelegate);
        thread.Start();
    }

    public void Run(CancellationTokenSource cancellationTokenSrc)
    {
        while (!cancellationTokenSrc.IsCancellationRequested)
        {
            if (!_chatClient._isConnected) continue;

            try
            {
                var buffer = new byte[1_024];
                var received = _socket.Receive(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"\n{response}");

                if (_chatClient.GetUserName() != null)
                {
                    Console.WriteLine($"[{_chatClient.GetUserName()}]: ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from the server: {ex}");
                break;
            }
        }
    }
}