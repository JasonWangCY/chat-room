namespace ChatRoom.Client.Services.Interfaces;

public interface IChatClient
{
    void Execute(int port, CancellationTokenSource cancellationTokenSrc);
    void SetUserName(string userName);
    string GetUserName();
}
