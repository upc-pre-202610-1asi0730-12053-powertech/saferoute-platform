using Powertech.Platform.Shared.Domain.Model.Events;
using Cortex.Mediator.Notifications;

/// AGREGADO NO EJECUTA COMMADS, SOLO LOS RECIBE Y
/// QUIEN LOS EJECUTA SON LOS HANDLER EN LOS COMMAND SERVICES EN LA CAPA DE APPLICATION

namespace Powertech.Platform.Shared.Application.Internal.EventHandlers;


public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}