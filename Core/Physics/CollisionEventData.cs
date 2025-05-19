using SiphoEngine.Physics;

namespace SiphoEngine.Core.Physics
{
    public readonly struct CollisionEventData
    {
        public readonly Collider Other;
        public readonly CollisionEventType EventType;

        public CollisionEventData(Collider other, CollisionEventType eventType)
        {
            Other = other;
            EventType = eventType;
        }
    }
}
