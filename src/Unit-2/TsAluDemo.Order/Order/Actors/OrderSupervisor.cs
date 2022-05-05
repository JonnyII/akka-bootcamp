using CargoSupport.Akka.Typed.Actors;
using CargoSupport.Akka.Typed.Messages;

namespace TsAluDemo.Order.Order.Actors;

internal static class OrderSupervisor
{
    public record Events : FrameworkMessages.Event
    {
        public record Created : Events;
    }

    public abstract record Commands : FrameworkMessages.Command
    {

    }
    public class Actor : EventRelayActor<Commands, Events>
    {
        public Actor()
        {

        }
    }
}
