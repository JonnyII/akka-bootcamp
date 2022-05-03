namespace CargoSupport.Akka.Typed.Exceptions;

public class UnhandledMessageException : Exception
{
    public UnhandledMessageException(object rawMessage) :
        base($"A Message was not handled by it's actor: {rawMessage}")
    { }
}