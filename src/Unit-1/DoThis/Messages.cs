namespace WinTail;
public class Messages
{
    #region Neutral/System messages

    public record ContinueProcessing();

    #endregion

    #region Success messages

    public record InputSuccess(string Reason);

    #endregion

    #region Error messages
    public record InputError(string Reason);

    public record NullInputError(string Reason) : InputError(Reason);

    public record ValidationError(string Reason) : InputError(Reason);

    #endregion
}
