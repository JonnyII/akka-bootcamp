namespace CargoSupport.Akka.Typed.Messages;

/// <summary>
///     a message which is not designated to a single actor.<br />
///     should only be used when you have an n:m relationship between Parent and Child actor types <br />
///     if you want to implement a baseClass, you might want to think about doing this via interface
/// </summary>
[Obsolete("this is most likely Unnecessary and an anti-pattern. ")]
public interface IMultiActorMessage
{
}