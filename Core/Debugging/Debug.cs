using System.Diagnostics;
using System.IO.Pipes;
namespace SiphoEngine.Core.Debugging
{

    public static class Debug
    {

        private static NamedPipeServerStream _pipe;
        private static StreamWriter _writer;
        private static Thread _pipeThread;
        private static Process _consoleProcess;

        private const string PIPE_NAME = "sipho_debug_pipe";

#if DEBUG
        internal static void Initialize()
        {
            LaunchDebugConsole();
            _pipe = new NamedPipeServerStream(
                PIPE_NAME,
                PipeDirection.Out,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous
            );

            _pipeThread = new Thread(WaitForConnection);
            _pipeThread.Start();
        }

        private static void LaunchDebugConsole()
        {
            try
            {
                var consolePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SiphoEngineDebugConsole.exe");
                if (File.Exists(consolePath))
                {
                    _consoleProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = consolePath,
                            WorkingDirectory = Path.GetDirectoryName(consolePath),
                            UseShellExecute = false,
                            CreateNoWindow = false
                        },
                        EnableRaisingEvents = true
                    };
                    _consoleProcess.Start();
                }
                else
                {
                    Console.WriteLine($"Debug console not found at: {consolePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to launch debug console: {ex}");
            }
        }


        internal static void EnableGlobalExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = (Exception)e.ExceptionObject;
                Error($"CRASH: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");

                Thread.Sleep(1000);
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Error($"ASYNC CRASH: {e.Exception}");
                e.SetObserved();
            };
        }

        private static void WaitForConnection()
        {
            try
            {
                _pipe.WaitForConnection();
                _writer = new StreamWriter(_pipe) { AutoFlush = true };
                Log("[Debug] Connected to console.");
            }
            catch
            {
                Shutdown();
            }
        }

#endif

        public static void Log(object message)
        {

            SendMessage("LOG", message, ConsoleColor.White);
        }

        public static void Warn(object message)
        {
            SendMessage("WARN", message, ConsoleColor.Yellow);
        }

        public static void Error(object message)
        {
            SendMessage("ERROR", message, ConsoleColor.Red);
        }

        private static void SendMessage(string type, object message, ConsoleColor color)
        {
#if DEBUG
            if (_writer == null || !_pipe.IsConnected)
                return;

            string formattedMessage = $"[{DateTime.Now:HH:mm:ss}] [{type}] {message}";
            try
            {
                _writer.WriteLine(formattedMessage);
            }
            catch
            {
                Shutdown();
            }
#endif
        }
#if DEBUG
        internal static void Shutdown()
        {
            try
            {
                _writer?.Dispose();
                _pipe?.Dispose();

                if (_consoleProcess != null && !_consoleProcess.HasExited)
                {
                    _consoleProcess.CloseMainWindow();
                    _consoleProcess.Kill();
                    _consoleProcess.Dispose();
                }
            }
            catch { /* Игнорируем ошибки при завершении */ }
        }
#endif
    }
}
