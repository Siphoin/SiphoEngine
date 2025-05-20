namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitForAny : ICoroutineYield
    {
        private AsyncCoroutine[] Coroutines { get; }
        private ICoroutineRunner Runner { get; }
        public WaitForAny(ICoroutineRunner runner,  params AsyncCoroutine[] coroutines)
        {
            Coroutines = coroutines;
            Runner = runner;
        }

        public bool IsDone(float deltaTime)
        {
            bool any = false;

            foreach (var coroutine in Coroutines)
            {
                if (!Runner.HasCoroutine(coroutine.Id))
                {
                    any = true;
                }
            }

            return any;
        }
    }
}