using Microsoft.AspNetCore.SignalR;

public class NotificationHub : Hub
{
    public const string RECEIVE_LOG_EVENT = "ReceiveLog";
}