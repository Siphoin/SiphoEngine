namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitForAll : ICoroutineYield
    {
        private AsyncCoroutine[] Coroutines { get; }
        private ICoroutineRunner Runner { get; }
        public WaitForAll(ICoroutineRunner runner, params AsyncCoroutine[] coroutines)
        {
            Coroutines = coroutines;
            Runner = runner;
        }

        public bool IsDone(float deltaTime)
        {
            int count = Coroutines.Length;
            int i = 0;
            bool any = false;

            foreach (var coroutine in Coroutines)
            {
                if (!Runner.HasCoroutine(coroutine.Id))
                {
                   i++;
                }
            }

            return i == count;
        }
    }
}