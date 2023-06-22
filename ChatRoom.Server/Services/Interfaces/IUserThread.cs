namespace ChatRoom.Server.Services.Interfaces;

public interface IUserThread
{
    void Run();
    Task SendMessageAsync(string message);
}
