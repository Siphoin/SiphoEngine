using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SiphoEngine.Core
{
    public static class Time
    {
        public static float DeltaTime { get; private set; }
        internal static float FixedDeltaTime { get; set; } = 0.01666667f; // 1/60 секунды
        public static float TimeScale { get; set; } = 1f;
        public static float TimeSinceStartup { get; private set; }

        private static Stopwatch _stopwatch = Stopwatch.StartNew();
        private static long _lastTick;

        static Time()
        {
            PreciseTimer.Initialize();
            _lastTick = _stopwatch.ElapsedTicks;
        }

        internal static void Update()
        {
            long currentTick = _stopwatch.ElapsedTicks;
            DeltaTime = Math.Clamp(
                (currentTick - _lastTick) / (float)Stopwatch.Frequency,
                0.0001f,
                0.1f
            ) * TimeScale;

            _lastTick = currentTick;
            TimeSinceStartup += DeltaTime;
        }
    }

    internal static class PreciseTimer
    {
        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint period);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint period);

        private static bool _initialized = false;

        internal static void Initialize()
        {
            if (!_initialized)
            {
                timeBeginPeriod(1);
                _initialized = true;
                AppDomain.CurrentDomain.ProcessExit += (s, e) => timeEndPeriod(1);
            }
        }
    }
}