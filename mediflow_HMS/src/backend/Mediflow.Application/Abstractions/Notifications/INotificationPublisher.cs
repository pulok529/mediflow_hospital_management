namespace Mediflow.Application.Abstractions.Notifications;

public sealed record LiveNotification(string Channel, string Message, DateTime AtUtc, object? Payload = null);

public interface INotificationPublisher
{
    Task PublishAsync(LiveNotification notification, CancellationToken cancellationToken = default);
}
