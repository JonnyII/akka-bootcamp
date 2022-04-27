using System;

using Akka.Actor;
using CargoSupport.Akka.Typed;
using IConsoleWriterActorRef = CargoSupport.Akka.Typed.IActorRef<WinTail.ConsoleWriterMessage>;

namespace WinTail;

public record TailCoordinatorMessage : ActorMessage { internal TailCoordinatorMessage() { } }
public class TailCoordinatorActor : Actor<TailCoordinatorActor, TailCoordinatorMessage>
{
    public class Messages
    {
        public record Start(string FilePath, IConsoleWriterActorRef ReporterActor) : TailCoordinatorMessage;
        public record StopTail(string FilePath) : TailCoordinatorMessage;
    }
    protected override void OnReceive(TailCoordinatorMessage message)
    {
        switch (message)
        {
            case Messages.Start(var filePath, var reporterActor):
                Context.ActorOf<TailActor, TailMessage>(TypedProps.Create(
                    () => new TailActor(reporterActor, filePath)));
                return;
        }
    }

    protected override SupervisorStrategy SupervisorStrategy()
    {
        return new OneForOneStrategy(
            10,
            TimeSpan.FromSeconds(30),
            ex => ex switch
                {
                    ArithmeticException => Directive.Resume,
                    UnhandledMessageException => Directive.Resume,
                    NotSupportedException => Directive.Stop,
                    _ => Directive.Restart
                }
            );
    }
}
