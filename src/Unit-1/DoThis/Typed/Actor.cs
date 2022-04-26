using System;

using Akka.Actor;

namespace WinTail.Typed;

public abstract class Actor<TThis> : UntypedActor
    where TThis : Actor<TThis>
{
    internal Actor() { }
}

public abstract class Actor<TThis, TMessageBase> : Actor<TThis>
    where TThis : Actor<TThis, TMessageBase>
{
    private static void Receiver(object rawMessage, Action<TMessageBase> handler)
    {
        if (rawMessage is TMessageBase typedMessage)
            handler(typedMessage);
    }

    protected override void OnReceive(object rawMessage)
        => Receiver(rawMessage, OnReceive);

    public void Become(Action<TMessageBase> newReceiver)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver));

    public void BecomeStacked(Action<TMessageBase> newReceiver)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver));

    protected abstract void OnReceive(TMessageBase message);
}