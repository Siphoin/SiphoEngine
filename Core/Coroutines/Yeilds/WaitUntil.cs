namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitUntil : ICoroutineYield
    {
        public Func<bool> Predicate { get; }

        public WaitUntil(Func<bool> predicate)
            => Predicate = predicate;

        public bool IsDone(float deltaTime)
        {
            return Predicate?.Invoke() ?? true;
        }
    }

}
