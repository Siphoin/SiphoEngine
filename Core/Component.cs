namespace SiphoEngine.Core
{
    public abstract class Component : Object
    {
        public GameObject GameObject { get; internal set; }
        public Transform Transform => GameObject?.Transform;
    }
}
