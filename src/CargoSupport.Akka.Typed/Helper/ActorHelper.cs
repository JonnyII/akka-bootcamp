using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Helper;


public static class ActorHelper
{
    public static string GetDefaultName<TActor>()
        where TActor : ActorBase
    {
        var type = typeof(TActor);
        return type.DeclaringType?.Name ?? type.Name.Replace("Actor", "");
    }

    public static ICancelable ScheduleTellRepeatedlyCancelable<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
            ICanTell<TMessage> receiver, TMessage command)
        where TMessage : FrameworkMessages.Command
        => scheduler.ScheduleTellRepeatedlyCancelable(initialDelay, interval, receiver.Native, command, ActorRefs.Nobody);

    public static void ScheduleTellRepeatedly<TCommand>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
            ICanTell<TCommand> receiver, TCommand command)
        where TCommand : FrameworkMessages.Command
        => scheduler.ScheduleTellRepeatedly(initialDelay, interval, receiver.Native, command, ActorRefs.Nobody);

    public static void ScheduleTellRepeatedly<TCommand>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
        ICanTell<TCommand> receiver, TCommand command, IActorRef<TCommand>? sender, ICancelable? cancelable = null)
        where TCommand : FrameworkMessages.Command
        => scheduler.ScheduleTellRepeatedly(initialDelay, interval, receiver.Native, command, sender?.Native ?? ActorRefs.Nobody, cancelable);

    public static ICancelable ScheduleTellOnceCancelable<TCommand>(
        this IScheduler scheduler, TimeSpan initialDelay,
            ICanTell<TCommand> receiver, TCommand command)
        where TCommand : FrameworkMessages.Command
        => scheduler.ScheduleTellOnceCancelable(initialDelay, receiver.Native, command, ActorRefs.Nobody);

    public static void ScheduleTellOnce<TCommand>(
        this IScheduler scheduler, TimeSpan initialDelay,
            ICanTell<TCommand> receiver, TCommand command)
        where TCommand : FrameworkMessages.Command
        => scheduler.ScheduleTellOnce(initialDelay, receiver.Native, command, ActorRefs.Nobody);

}