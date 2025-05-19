using SiphoEngine.Core;
using SiphoEngine.Core.Physics;
using SiphoEngine.Physics;

public abstract class Collider : Component
{
    public event Action<CollisionEventData> OnCollisionEvent;
    public bool IsTrigger { get; set; }

    private HashSet<Collider> _currentCollisions = new HashSet<Collider>();
    private HashSet<Collider> _previousCollisions = new HashSet<Collider>();

    public abstract bool CheckCollision(Collider other, ref CollisionInfo info);

    protected bool CheckCollisionTypes(Collider a, Collider b, ref CollisionInfo info)
    {
        if (a is BoxCollider boxA && b is BoxCollider boxB)
            return boxA.CheckBoxCollision(boxB, ref info);

        if (a is CircleCollider circleA && b is CircleCollider circleB)
            return circleA.CheckCircleCollision(circleB, ref info);

        if (a is BoxCollider box && b is CircleCollider circle)
            return box.CheckCircleCollision(circle, ref info);

        if (a is CircleCollider circle2 && b is BoxCollider box2)
            return box2.CheckCircleCollision(circle2, ref info);

        return false;
    }

    public void UpdateCollisionEvents()
    {
        // Check for exit events
        foreach (var other in _previousCollisions)
        {
            if (!_currentCollisions.Contains(other))
            {
                OnCollisionEvent?.Invoke(new CollisionEventData(other, CollisionEventType.Exit));
            }
        }

        // Check for enter and stay events
        foreach (var other in _currentCollisions)
        {
            if (!_previousCollisions.Contains(other))
            {
                OnCollisionEvent?.Invoke(new CollisionEventData(other, CollisionEventType.Enter));
            }
            else
            {
                OnCollisionEvent?.Invoke(new CollisionEventData(other, CollisionEventType.Stay));
            }
        }

        // Update previous collisions
        _previousCollisions = new (_currentCollisions);
        _currentCollisions.Clear();
    }

    public void AddCurrentCollision(Collider other)
    {
        _currentCollisions.Add(other);
    }
}