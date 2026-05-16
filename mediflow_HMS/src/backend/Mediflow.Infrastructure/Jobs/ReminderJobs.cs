using Mediflow.Application.Abstractions.Notifications;

namespace Mediflow.Infrastructure.Jobs;

public sealed class ReminderJobs(INotificationPublisher publisher)
{
    public async Task SendQueueReminder()
    {
        await publisher.PublishAsync(new LiveNotification("queue", "Queue reminder: review waiting patients", DateTime.UtcNow));
    }

    public async Task SendDischargeReminder()
    {
        await publisher.PublishAsync(new LiveNotification("discharge", "Discharge reminder: pending final clearances", DateTime.UtcNow));
    }
}
