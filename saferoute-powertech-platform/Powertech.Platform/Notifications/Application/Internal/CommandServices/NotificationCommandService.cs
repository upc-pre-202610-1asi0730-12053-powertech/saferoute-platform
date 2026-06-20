using Powertech.Platform.Notifications.Application.CommandServices;
using Powertech.Platform.Notifications.Domain.Model;
using Powertech.Platform.Notifications.Domain.Model.Aggregates;
using Powertech.Platform.Notifications.Domain.Model.Commands;
using Powertech.Platform.Notifications.Domain.Repositories;
using Powertech.Platform.Shared.Application.Model;
using Powertech.Platform.Shared.Domain.Repositories;

namespace Powertech.Platform.Notifications.Application.Internal.CommandServices;

public class NotificationCommandService(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork) : INotificationCommandService
{
    public async Task<Result<Notification>> Handle(CreateNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var notification = new Notification(command);
        
        await notificationRepository.AddAsync(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result<Notification>.Success(notification);
    }

    public async Task<Result<Notification>> Handle(DispatchNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.FindByIdAsync(command.NotificationId);
        if (notification is null) return Result<Notification>.Failure(NotificationError.NotificationNotFound, "Notification not found");
        
        notification.Dispatch();
        
        notificationRepository.Update(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result<Notification>.Success(notification);
    }

    public async Task<Result<Notification>> Handle(MarkNotificationDeliveredCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.FindByIdAsync(command.NotificationId);
        if (notification is null) return Result<Notification>.Failure(NotificationError.NotificationNotFound, "Notification not found");
        
        notification.MarkDelivered();
        
        notificationRepository.Update(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result<Notification>.Success(notification);
    }

    public async Task<Result<Notification>> Handle(TriggerAlertCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.FindByIdAsync(command.NotificationId);
        if (notification is null) return Result<Notification>.Failure(NotificationError.NotificationNotFound, "Notification not found");
        
        notification.AddAlert(command.Panic);
        
        notificationRepository.Update(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result<Notification>.Success(notification);
    }

    public async Task<Result<Notification>> Handle(PublishAnnouncementCommand command, CancellationToken cancellationToken = default)
    {
        var notification = await notificationRepository.FindByIdAsync(command.NotificationId);
        if (notification is null) return Result<Notification>.Failure(NotificationError.NotificationNotFound, "Notification not found");
        
        notification.AddAnnouncement(command.RouteId, command.Message);
        
        notificationRepository.Update(notification);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result<Notification>.Success(notification);
    }
}
