using Akka.Actor;

namespace CargoSupport.Akka.Typed;


public static class ActorHelper
{
    public static string GetDefaultName<TActor>()
        where TActor : ActorBase
        => typeof(TActor).Name.Replace("Actor", "");

    public static ICancelable ScheduleTellRepeatedlyCancelable<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : ActorMessage
        => scheduler.ScheduleTellRepeatedlyCancelable(initialDelay, interval, receiver.Native, message, ActorRefs.Nobody);

    public static void ScheduleTellRepeatedly<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay, TimeSpan interval,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : ActorMessage
        => scheduler.ScheduleTellRepeatedly(initialDelay, interval, receiver.Native, message, ActorRefs.Nobody);

    public static ICancelable ScheduleTellOnceCancelable<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : ActorMessage
        => scheduler.ScheduleTellOnceCancelable(initialDelay, receiver.Native, message, ActorRefs.Nobody);

    public static void ScheduleTellOnce<TMessage>(
        this IScheduler scheduler, TimeSpan initialDelay,
            ICanTell<TMessage> receiver, TMessage message)
        where TMessage : ActorMessage
        => scheduler.ScheduleTellOnce(initialDelay, receiver.Native, message, ActorRefs.Nobody);

}