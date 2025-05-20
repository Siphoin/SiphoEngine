namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitOtherCoroutine : ICoroutineYield
    {
        private ICoroutineRunner _runner;
        private Guid _guid;
       

        public WaitOtherCoroutine(ICoroutineRunner runner, AsyncCoroutine coroutine)
        {
            _runner = runner;
            _guid = coroutine.Id;
        }

        public bool IsDone(float deltaTime)
        {
            return _runner.HasCoroutine(_guid) == false;
        }
    }
}