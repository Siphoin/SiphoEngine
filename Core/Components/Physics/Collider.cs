using SiphoEngine.Core;


namespace SiphoEngine.Physics
{
    public abstract class Collider : Component
    {
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
    }
}
