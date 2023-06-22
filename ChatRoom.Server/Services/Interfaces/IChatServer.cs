namespace ChatRoom.Server.Services.Interfaces;

public interface IChatServer
{
    Task Execute(int port, CancellationTokenSource cancellationTokenSrc);
    Task BroadcastAsync(string message, UserThread author);
    void AddUserName(string userName);
    void RemoveUser(string userName, UserThread user);
    HashSet<string> GetUserNames();
    bool HasUsers();
}
