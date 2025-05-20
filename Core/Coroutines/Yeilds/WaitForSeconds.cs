using SiphoEngine.Core.Debugging;

namespace SiphoEngine.Core.Coroutines.Yeilds
{
    public struct WaitForSeconds : ICoroutineYield
    {
        private float _remaining;
        private readonly float _target;

        public WaitForSeconds(float seconds)
        {
            _target = seconds;
        }

        public bool IsDone(float deltaTime)
        {
            _remaining += deltaTime;
            bool isDone = _remaining >= _target;
            if (isDone)
            {
                _remaining = 0;
            }
            return isDone;
        }
    }

}
