using System;

using Akka.Actor;

namespace WinTail.Typed;

/// <summary>
/// a message which is not designated to a single actor.<br/>
/// should only be used when you have an n:m relationship between Parent and Child actor types <br/>
/// if you want to implement a baseClass, you might want to think about doing this via interface
/// </summary>
public record MultiActorMessage;

public abstract class Actor<TThis> : UntypedActor
    where TThis : Actor<TThis>
{
    internal Actor() { }
}

public abstract class Actor<TThis, TMessageBase> : Actor<TThis>
    where TThis : Actor<TThis, TMessageBase>
{
    private static void Receiver(object rawMessage, Action<TMessageBase> handler, Action<MultiActorMessage> genericReceiver)
    {
        switch (rawMessage)
        {
            case TMessageBase typedMessage:
                handler(typedMessage);
                break;
            case MultiActorMessage genericMessage:
                genericReceiver(genericMessage);
                break;
        }
    }

    protected override void OnReceive(object rawMessage)
        => Receiver(rawMessage, OnReceive, OnReceive);
    protected virtual void OnReceive(MultiActorMessage multiActorMessage) { }
    public void Become(Action<TMessageBase> newReceiver, Action<MultiActorMessage> genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    public void BecomeStacked(Action<TMessageBase> newReceiver, Action<MultiActorMessage> genericReceiver = null)
        => base.Become(rawMessage => Receiver(rawMessage, newReceiver, genericReceiver));

    protected abstract void OnReceive(TMessageBase message);
}