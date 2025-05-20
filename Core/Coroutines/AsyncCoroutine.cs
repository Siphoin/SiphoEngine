using SiphoEngine.Core.Coroutines.Yeilds;
using SiphoEngine.Core.Coroutines;
using SiphoEngine.Core.Debugging;

public struct AsyncCoroutine
{
    
    private IEnumerator<ICoroutineYield> _enumerator;
   
    private ICoroutineYield? _currentYield;

    public Guid Id { get; private set; }


    internal AsyncCoroutine(IEnumerator<ICoroutineYield> enumerator)
    {
        Id  = Guid.NewGuid();
        _enumerator = enumerator;
        _enumerator.MoveNext();
        _currentYield = _enumerator.Current;
    }

    public bool IsDone { get; private set; }
    public IEnumerator<ICoroutineYield> Enumerator => _enumerator;

    internal bool Update(float deltaTime)
    {
        if (_currentYield != null)
        {
            bool isDone = _currentYield.IsDone(deltaTime);
            if (isDone)
            {
                if (_enumerator.MoveNext())
                {
                    _currentYield = _enumerator.Current;
                    return true;
                }

                else
                {
                    IsDone = true;
                    return false;
                }
            }
        }
        
        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is AsyncCoroutine other && Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}