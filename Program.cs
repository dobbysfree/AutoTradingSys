using Akka.Actor;
using Serilog;
using Serilog.Events;

namespace AutoTradingSys
{
    internal static class Program
    {
        // Provides logging to various sinks such as  files, console, and more
        public static ILogger ILog { get; set; }
        // Process messages asynchronously
        public static ActorSystem ActSys { get; set; }
        // For waiting process
        public static TaskCompletionSource<bool> Waiting { get; set; } = new TaskCompletionSource<bool>();

        [STAThread]
        static void Main(string[] args)
        {
            // Exclude weekends
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;

           ILog = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fffff}] {Message:lj}{NewLine}")
               .WriteTo.File(
                   Application.StartupPath + "/logs/log_.txt",
                   rollingInterval: RollingInterval.Day,
                   outputTemplate: "[{Timestamp:HH:mm:ss.fffff}] {Message:lj}{NewLine}",
                   fileSizeLimitBytes: 50_000_000,
                   rollOnFileSizeLimit: true,
                   shared: true,
                   flushToDiskInterval: TimeSpan.FromSeconds(1),
                   retainedFileCountLimit: 500)
               .CreateLogger();

            // Check input parameters
            if (args.Length != 2)
            {
                ILog.Warning($"Args {args.Length}");
                Application.Exit();
            }

            ActSys = ActorSystem.Create("ActSys");

            for (int i = 0; i < args.Length; i++)
            {
                var kv = args[i].Split(':');
                App.IConf[kv[0]] = kv[1];
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new App());
        }
    }
}