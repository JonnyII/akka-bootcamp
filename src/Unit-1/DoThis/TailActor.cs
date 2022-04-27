using System;
using System.IO;
using System.Text;

using WinTail.Typed;

namespace WinTail;
public record TailMessage : ActorMessage { internal TailMessage() { } }

public class TailActor : Actor<TailActor, TailMessage, TailCoordinatorActor, TailCoordinatorMessage>, IDisposable
{
    public class Messages
    {
        public record FileWrite(string FileName) : TailMessage;
        public record FileError(string FileName, string Reason) : TailMessage;
        public record InitialRead(string FileName, string Text) : TailMessage;
    }

    public class ConsoleWriterExtensions
    {
        public record FileUpdate(string Reason) : ConsoleWriterActor.Messages.Success(Reason);
    }
    private readonly string _filePath;
    private readonly IActorRef<ConsoleWriterMessage> _reporterActor;
    private readonly FileObserver _observer;
    private readonly Stream _fileStream;
    // defined here since we only want to read the changes since the last update
    // file is expected to be a append-only log
    private readonly StreamReader _fileStreamReader;

    public TailActor(IActorRef<ConsoleWriterMessage> reporterActor, string filePath)
    {
        _reporterActor = reporterActor;
        _filePath = filePath;

        _observer = new(Self, Path.GetFullPath(_filePath));
        _observer.Start();


        _fileStream = new FileStream(
            Path.GetFullPath(_filePath),
            FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        _fileStreamReader = new(_fileStream, Encoding.UTF8);

        var text = _fileStreamReader.ReadToEnd();
        Self.Tell(new Messages.InitialRead(_filePath, text));
    }
    protected override void OnReceive(TailMessage message)
    {
        switch (message)
        {
            case Messages.InitialRead(_, var initialText):
                _reporterActor.Tell(new ConsoleWriterExtensions.FileUpdate(initialText));
                return;
            case Messages.FileWrite:
                var text = _fileStreamReader.ReadToEnd();
                if (string.IsNullOrEmpty(text))
                    return;
                _reporterActor.Tell(new ConsoleWriterExtensions.FileUpdate(text));
                return;
            case Messages.FileError(_, var reason):
                _reporterActor.Tell(new ConsoleWriterActor.Messages.Error($"Tail error: {reason}"));
                return;
        }
    }

    public void Dispose()
    {
        _fileStream?.Dispose();
        _fileStreamReader?.Dispose();
        _observer?.Dispose();
        GC.SuppressFinalize(this);
    }
}

