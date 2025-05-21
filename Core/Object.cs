namespace SiphoEngine.Core
{
    public abstract class Object
    {
        public abstract void Destroy();

        public static bool operator ==(Object a, Object b)
        {
            if (ReferenceEquals(a, null)) 
            {
                return ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null)) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Object a, Object b)
        {
            return !(a == b);
        }

        public static bool operator true(Object obj)
        {
            return obj != null;
        }

        public static bool operator false(Object obj)
        {
            return obj == null;
        }

        public static implicit operator bool(Object obj)
        {
            return obj != null;
        }
    }
}
