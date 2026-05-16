using Mediflow.Application.Abstractions.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Mediflow.Api.Notifications;

public sealed class NotificationHub : Hub { }

public sealed class HubNotificationPublisher(IHubContext<NotificationHub> hubContext) : INotificationPublisher
{
    public async Task PublishAsync(LiveNotification notification, CancellationToken cancellationToken = default)
    {
        await hubContext.Clients.All.SendAsync("notification", notification, cancellationToken);
    }
}
