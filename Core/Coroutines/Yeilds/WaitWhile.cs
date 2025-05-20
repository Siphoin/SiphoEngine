namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitWhile : ICoroutineYield
    {
        public Func<bool> Predicate { get; }

        public WaitWhile(Func<bool> predicate)
            => Predicate = predicate;

        public bool IsDone(float deltaTime)
        {
            return !Predicate?.Invoke() ?? true;
        }
    }

}
