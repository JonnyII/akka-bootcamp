
using Akka.Actor;

namespace ChartApp;

internal static class Program
{
    /// <summary>
    /// ActorSystem we'll be using to publish data to charts
    /// and subscribe from performance counters
    /// </summary>
    public static ActorSystem ChartActors = ActorSystem.Create("ChartActors");

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Main());
    }
}
