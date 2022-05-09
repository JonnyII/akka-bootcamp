namespace TsAluDemo.Order.Order.Actors;

internal class Favorite
{
    public record Commands : OrderSupervisor.Commands
    {
        public record AddToCart;
    }

    public record Events : OrderSupervisor.Events
    {

    }

    public class Actor : EventActor<Commands, Events>
    {

    }
}
