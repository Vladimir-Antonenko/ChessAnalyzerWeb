using Microsoft.AspNetCore.SignalR;

namespace ChessAnalyzerApi.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    //public override async Task OnConnectedAsync()
    //{
    //    await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел");
    //    await base.OnConnectedAsync();
    //}
    //public override async Task OnDisconnectedAsync(Exception? exception)
    //{
    //    await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул");
    //    await base.OnDisconnectedAsync(exception);
    //}
}