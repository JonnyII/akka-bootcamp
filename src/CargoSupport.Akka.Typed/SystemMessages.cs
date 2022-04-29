using NativePoisonPill = Akka.Actor.PoisonPill;

namespace CargoSupport.Akka.Typed;
public static class SystemMessages
{
    public abstract record SystemMessage
    {
        internal SystemMessage() { }
        public abstract object GetNative();
    }

    public static PoisonPillClass PoisonPill = new();
    public record PoisonPillClass : SystemMessage
    {
        internal PoisonPillClass() { }
        public override NativePoisonPill GetNative()
            => NativePoisonPill.Instance;
    }
}
