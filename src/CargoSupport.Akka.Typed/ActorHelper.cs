using Akka.Actor;

namespace CargoSupport.Akka.Typed;

public static class ActorHelper
{
    public static string GetDefaultName<TActor>()
        where TActor : UntypedActor
        => typeof(TActor).Name.Replace("Actor", "");
}