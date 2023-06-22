using System.Net.Sockets;
using System.Text;

namespace ChatRoom.Client.Services;

public class WriteThread
{
    private readonly Socket _socket;
    private readonly ChatClient _chatClient;

    private StreamWriter _writer = null!;

    public WriteThread(Socket socket, ChatClient chatClient)
    {
        _socket = socket;
        _chatClient = chatClient;

        InitializeConnection();
    }

    private void InitializeConnection()
    {
        try
        {
            var outputStream = new NetworkStream(_socket, FileAccess.Write);
            _writer = new StreamWriter(outputStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting input stream: {ex}");
        }
    }

    public void Start(CancellationTokenSource cancellationTokenSrc)
    {
        var test = Run;
        var threadDelegate = new ThreadStart(() => Run(cancellationTokenSrc));
        var thread = new Thread(threadDelegate);
        thread.Start();
    }

    public void Run(CancellationTokenSource cancellationTokenSrc)
    {
        Console.WriteLine("\nEnter your name: ");
        var userName = Console.ReadLine();
        while (string.IsNullOrEmpty(userName))
        {
            Console.WriteLine("\nThe name cannot be empty. Enter your name again: ");
            userName = Console.ReadLine();
        }

        _chatClient.SetUserName(userName);
        var messageBytes = Encoding.UTF8.GetBytes(userName);
        _socket.Send(messageBytes, SocketFlags.None);

        var text = string.Empty;
        while (!string.Equals(text, "quit"))
        {
            Console.WriteLine($"[{userName}]: ");
            text = Console.ReadLine();
            messageBytes = Encoding.UTF8.GetBytes(text);
            _socket.Send(messageBytes, SocketFlags.None);
        }

        try
        {
            _socket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to server: {ex}");
        }
    }
}