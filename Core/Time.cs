namespace SiphoEngine.Core
{
    public static class Time
    {
        public static float DeltaTime { get; private set; }
        public static float FixedDeltaTime { get; set; } = 0.02f;
        public static float TimeScale { get; set; } = 1f;
        public static float TimeSinceStartup { get; private set; }

        private static float _lastFrameTime;

        internal static void Update(float currentTime)
        {
            DeltaTime = (currentTime - _lastFrameTime) * TimeScale;
            TimeSinceStartup += DeltaTime;
            _lastFrameTime = currentTime;
        }
    }
}
