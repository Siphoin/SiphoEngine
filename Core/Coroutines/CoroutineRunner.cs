using SiphoEngine.Core.Coroutines.Yeilds;
using SiphoEngine.Core.Coroutines;
using SiphoEngine.Core.Debugging;
namespace SiphoEngine.Core.Components
{
    internal class CoroutineRunner : ICoroutineRunner
    {
        private readonly List<AsyncCoroutine> _activeCoroutines = new();
        private readonly List<AsyncCoroutine> _coroutinesToAdd = new();
        private bool _isUpdating;

        internal CoroutineRunner()
        {

        }

        internal AsyncCoroutine StartCoroutine(IEnumerator<ICoroutineYield> coroutine)
        {
            var asyncCoroutine = new AsyncCoroutine(coroutine);
            if (_isUpdating)
                _coroutinesToAdd.Add(asyncCoroutine);
            else
                _activeCoroutines.Add(asyncCoroutine);

            return asyncCoroutine;
        }


        internal void StopAllCoroutines()
        {
            _activeCoroutines.Clear();
            _coroutinesToAdd.Clear();
        }

        internal void StopCoroutine(ref AsyncCoroutine coroutine)
        {
            _activeCoroutines.Remove(coroutine);
        }

        internal void Update(float deltaTime)
        {
            _isUpdating = true;
            for (int i = 0; i < _activeCoroutines.Count; i++)
            {
                bool isUpdate = _activeCoroutines[i].Update(deltaTime);
                if (!isUpdate)
                {
                    Debug.Log($"{nameof(CoroutineRunner)}: stop coroutine call");
                    _activeCoroutines.RemoveAt(i);
                }
            }

            _isUpdating = false;
        }

        public void DelayAction(float delay, Action action)
        {
            StartCoroutine(DelayActionCoroutine(delay, action));
        }

        private IEnumerator<ICoroutineYield> DelayActionCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        public bool HasCoroutine(Guid guid)
        {
            foreach (var coroutine in _activeCoroutines)
            {
                if (coroutine.Id == guid)
                {
                    return true;
                }
            }

            return false;
        }
    }
}