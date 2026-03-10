using System.Reflection;

namespace SolarLiveStatusPanel
{
    internal static class Program
    {
        private static ILogger? Logger;

        private static string GetCodeVersion()
        {
            // do not move the GetExecutingAssembly call from here into a supporting DLL
            Assembly me = Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            return name.Version?.ToString() ?? "UNKNOWN";
        }


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var loggerFactory = new NLoggerLoggerFactory();
            Logger = loggerFactory.Logger;
            Logger.Info(() => $"SolarLiveStatusPanel, v{GetCodeVersion()}, Running on .NET CLR: {Environment.Version.ToString()}");

            SetupExceptionHandler();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1(Logger));

            Logger.Info(() => $"SolarLiveStatusPanel, **EXIT**");
        }

        private static void SetupExceptionHandler()
        {
            // Add handler for UI thread exceptions
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add handler for non-UI thread exceptions
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Add handler for background threads/tasks
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private static void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger?.LogException(() => "TaskSchedulerOnUnobservedTaskException", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception ?? new Exception("EXCEPTION NOT PROVIDED");
            Logger?.LogException(() => "CurrentDomain_UnhandledException", ex);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger?.LogException(() => "Application_ThreadException", e.Exception);
        }
    }
}