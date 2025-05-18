namespace SiphoEngine.Core
{
    public static class Time
    {
        public static float DeltaTime { get; private set; }
        public static float FixedDeltaTime { get; set; } = 0.02f;
        public static float TimeScale { get; set; } = 1f;
        public static float TimeSinceStartup { get; private set; }

        public static void Update(float rawDeltaTime)
        {
            DeltaTime = rawDeltaTime * TimeScale; // Убираем лишние вычисления
            TimeSinceStartup += DeltaTime;
        }
    }
}
