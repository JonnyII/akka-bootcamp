namespace TsAluDemo.Order.Order.Actors;

internal class Cart
{
    public record Commands : OrderSupervisor.Commands
    {

    }

    public record Events : OrderSupervisor.Events
    {

    }

    public class Actor : EventActor<Commands, Events>
    {

    }

}
