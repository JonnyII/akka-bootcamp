using Akka.Actor;

namespace CargoSupport.Akka.Typed;

public static class ActorHelper
{
    public static string GetDefaultName<TActor>()
        where TActor : ActorBase
        => typeof(TActor).Name.Replace("Actor", "");
}