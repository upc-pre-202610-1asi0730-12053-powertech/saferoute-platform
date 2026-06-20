using Powertech.Platform.Notifications.Domain.Model.Commands;
using Powertech.Platform.Notifications.Domain.Model.Entities;
using Powertech.Platform.Notifications.Domain.Model.ValueObjects;

namespace Powertech.Platform.Notifications.Domain.Model.Aggregates;

public partial class Notification
{
    public NotificationId Id { get; private set; }
    public NotificationId OrganizationId { get; private set; }
    public NotificationId ParentId { get; private set; }
    public NotificationId TripId { get; private set; }
    public NotificationCategory Category { get; private set; }
    public NotificationDeliveryState DeliveryState { get; private set; }
    public NotificationMessage Message { get; private set; }
    public DateTimeOffset SentAt { get; private set; }

    public ICollection<Alert> Alerts { get; } = new List<Alert>();
    public ICollection<Announcement> Announcements { get; } = new List<Announcement>();

    protected Notification()
    {
        Id = new NotificationId();
        OrganizationId = new NotificationId(Guid.Empty);
        ParentId = new NotificationId(Guid.Empty);
        TripId = new NotificationId(Guid.Empty);
        Category = new NotificationCategory(string.Empty);
        DeliveryState = new NotificationDeliveryState("Pending");
        Message = new NotificationMessage(string.Empty);
        SentAt = DateTimeOffset.UtcNow;
    }

    public Notification(CreateNotificationCommand command) : this()
    {
        OrganizationId = new NotificationId(command.OrganizationId);
        ParentId = new NotificationId(command.ParentId);
        TripId = new NotificationId(command.TripId);
        Category = new NotificationCategory(command.Category);
        Message = new NotificationMessage(command.Message);
        DeliveryState = new NotificationDeliveryState("Pending");
    }

    public void Dispatch()
    {
        DeliveryState = new NotificationDeliveryState("Dispatched");
    }

    public void MarkDelivered()
    {
        DeliveryState = new NotificationDeliveryState("Delivered");
    }

    public void AddAlert(bool panic)
    {
        Alerts.Add(new Alert(Id, panic));
    }
}
