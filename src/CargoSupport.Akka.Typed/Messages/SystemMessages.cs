using NativePoisonPill = Akka.Actor.PoisonPill;

namespace CargoSupport.Akka.Typed.Messages;
public static class SystemMessages
{
    public abstract record SystemCommand
    {
        internal SystemCommand() { }
        public abstract object GetNative();
    }

    public static PoisonPillClass PoisonPill = new();
    public record PoisonPillClass : SystemCommand
    {
        internal PoisonPillClass() { }
        public override NativePoisonPill GetNative()
            => NativePoisonPill.Instance;
    }
}

public static class FrameworkMessages
{
    /// <summary>
    /// incoming actor message (like a public method)
    /// </summary>
    public abstract record Command;

    /// <summary>
    /// outgoing event
    /// they have to be either sealed or abstract and only the final implementation may be passed as parameter to subscribe, otherwise the handler will not be called
    /// </summary>
    public abstract record Event;

    public abstract record SubscriptionUpdate
    {
        public abstract Type MessageType { get; }
    }

    public abstract record Subscribe : SubscriptionUpdate;
    public abstract record Unsubscribe : SubscriptionUpdate;
    public sealed record Subscribe<TEventMessage> : Subscribe
    {
        public override Type MessageType => typeof(TEventMessage);
    }
    public sealed record Unsubscribe<TEventMessage> : Subscribe
    {
        public override Type MessageType => typeof(TEventMessage);
    }
}
