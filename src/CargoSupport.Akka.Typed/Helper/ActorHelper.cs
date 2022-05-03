using Akka.Actor;

using CargoSupport.Akka.Typed.ActorRef;
using CargoSupport.Akka.Typed.Messages;

namespace CargoSupport.Akka.Typed.Helper;


public static class ActorHelper
{
    public static string GetDefaultName<TActor>()
        where TActor : ActorBase
        => typeof(TActor).Name.Replace("Actor", "");

    public static ICancelable ScheduleTellRepeatedlyCancelable<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : FrameworkMessages.ActorCommand
        => scheduler.ScheduleTellRepeatedlyCancelable(initialDelay, interval, receiver.Native, message, ActorRefs.Nobody);

    public static void ScheduleTellRepeatedly<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : FrameworkMessages.ActorCommand
        => scheduler.ScheduleTellRepeatedly(initialDelay, interval, receiver.Native, message, ActorRefs.Nobody);

    public static void ScheduleTellRepeatedly<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
        ICanTell<TMessage> receiver, TMessage message, IActorRef<TMessage>? sender, ICancelable? cancelable = null)
        where TMessage : FrameworkMessages.ActorCommand
        => scheduler.ScheduleTellRepeatedly(initialDelay, interval, receiver.Native, message, sender?.Native ?? ActorRefs.Nobody, cancelable);

    public static ICancelable ScheduleTellOnceCancelable<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : FrameworkMessages.ActorCommand
        => scheduler.ScheduleTellOnceCancelable(initialDelay, receiver.Native, message, ActorRefs.Nobody);

    public static void ScheduleTellOnce<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : FrameworkMessages.ActorCommand
        => scheduler.ScheduleTellOnce(initialDelay, receiver.Native, message, ActorRefs.Nobody);

}