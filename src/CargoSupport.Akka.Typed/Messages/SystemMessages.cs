using Akka.Actor;

using NativePoisonPill = Akka.Actor.PoisonPill;

namespace CargoSupport.Akka.Typed.Messages;

public static class SystemMessages
{
    public static PoisonPillClass PoisonPill = new();

    public abstract record SystemCommand
    {
        internal SystemCommand()
        {
        }

        public abstract object GetNative();
    }

    public record PoisonPillClass : SystemCommand
    {
        internal PoisonPillClass()
        {
        }

        public override NativePoisonPill GetNative()
        {
            return NativePoisonPill.Instance;
        }
    }
}

public static class FrameworkMessages
{
    /// <summary>
    ///     incoming actor message (like a public method)
    /// </summary>
    public abstract record Command;

    /// <summary>
    ///     outgoing event
    ///     they have to be either sealed or abstract and only the final implementation may be passed as parameter to
    ///     subscribe, otherwise the handler will not be called
    /// </summary>
    public abstract record Event;

    public abstract record SubscriptionUpdate<TThis>
        where TThis : SubscriptionUpdate<TThis>
    {
        public abstract Type MessageType { get; }

        /// <summary>
        ///     should only be used by a supervising actor to connect a event-receiver to a child actor
        ///     default is <see cref="ActorBase.Sender" />.Sender
        /// </summary>
        internal IActorRef? Receiver { get; init; }

        internal abstract TThis CloneWithReceiver(IActorRef? receiver);
    }

    public abstract record Subscribe : SubscriptionUpdate<Subscribe>
    {
        internal Subscribe(Type messageType)
        {
            MessageType = messageType;
        }

        public override Type MessageType { get; }
    }

    public abstract record Unsubscribe : SubscriptionUpdate<Unsubscribe>
    {
        internal Unsubscribe(Type messageType)
        {
            MessageType = messageType;
        }

        public override Type MessageType { get; }
    }

    public sealed record Subscribe<TEventMessage>() : Subscribe(typeof(TEventMessage))
    {
        internal override Subscribe CloneWithReceiver(IActorRef? receiver)
        {
            return new Subscribe<TEventMessage>
            {
                Receiver = receiver
            };
        }
    }

    public sealed record Unsubscribe<TEventMessage>() : Unsubscribe(typeof(TEventMessage))
    {
        internal override Unsubscribe CloneWithReceiver(IActorRef? receiver)
        {
            return new Unsubscribe<TEventMessage>
            {
                Receiver = receiver
            };
        }
    }
}