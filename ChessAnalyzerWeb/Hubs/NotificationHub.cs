using Domain.GameAggregate;
using Microsoft.AspNetCore.SignalR;

namespace ChessAnalyzerApi.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinGroup(string playerName, ChessPlatform platform)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{platform}{playerName}");
    }

    public async Task LeaveGroup(string playerName, ChessPlatform platform)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{platform}{playerName}");
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