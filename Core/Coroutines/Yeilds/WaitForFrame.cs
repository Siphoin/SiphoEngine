namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitForFrame : ICoroutineYield
    {
        private bool _framePassed;

        public bool IsDone(float deltaTime)
        {
            if (!_framePassed)
            {
                _framePassed = true;
                return false;
            }
            return true;
        }
    }

}
