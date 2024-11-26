using Microsoft.AspNetCore.SignalR;

namespace SimpleRDBMSRestfulAPI.Libs;

public class NotificationHub : Hub
{
    public const string RECEIVE_LOG_EVENT = "ReceiveLog";
}