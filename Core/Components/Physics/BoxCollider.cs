using SFML.System;
using SiphoEngine.Core;
using SiphoEngine.Core.Debugging;
using SiphoEngine.Core.Physics;
using SiphoEngine.MathExtensions;


namespace SiphoEngine.Physics
{
    public class BoxCollider : Collider
    {
        public Vector2f Size { get; set; } = new Vector2f(64, 64);
        public Vector2f Offset { get; set; }

        public override bool CheckCollision(Collider other, ref CollisionInfo info)
        {
#if DEBUG
            DebugDraw.DrawBoxCollider(Transform.Position, Size, Offset);
#endif

            return CheckCollisionTypes(this, other, ref info);
        }

        internal bool CheckBoxCollision(BoxCollider other, ref CollisionInfo info)
        {
            Vector2f aMin = GetWorldMin();
            Vector2f aMax = GetWorldMax();
            Vector2f bMin = other.GetWorldMin();
            Vector2f bMax = other.GetWorldMax();

            bool collision = aMin.X < bMax.X && aMax.X > bMin.X &&
                            aMin.Y < bMax.Y && aMax.Y > bMin.Y;

            if (collision)
            {
                Vector2f overlap = new Vector2f(
                    Math.Min(aMax.X - bMin.X, bMax.X - aMin.X),
                    Math.Min(aMax.Y - bMin.Y, bMax.Y - aMin.Y)
                );

                if (overlap.X < overlap.Y)
                {
                    info.Normal = new Vector2f(aMax.X > bMax.X ? 1 : -1, 0);
                    info.Depth = overlap.X;
                }
                else
                {
                    info.Normal = new Vector2f(0, aMax.Y > bMax.Y ? 1 : -1);
                    info.Depth = overlap.Y;
                }

                info.A = this.GameObject.GetComponent<Rigidbody>();
                info.B = other.GameObject.GetComponent<Rigidbody>();

            }
            return collision;
        }

        internal bool CheckCircleCollision(CircleCollider other, ref CollisionInfo info)
        {
            Vector2f closestPoint = Transform.Position + Offset;
            Vector2f circleCenter = other.Transform.Position + other.Offset;
            Vector2f halfSize = Size / 2;

            closestPoint.X = Math.Clamp(circleCenter.X, closestPoint.X - halfSize.X, closestPoint.X + halfSize.X);
            closestPoint.Y = Math.Clamp(circleCenter.Y, closestPoint.Y - halfSize.Y, closestPoint.Y + halfSize.Y);

            float distance = closestPoint.Distance(circleCenter);
            bool collision = distance < other.Radius;

            if (collision)
            {
                info.Normal = (circleCenter - closestPoint).Normalized();
                info.Depth = other.Radius - distance;
                info.A = this.GameObject.GetComponent<Rigidbody>();
                info.B = other.GameObject.GetComponent<Rigidbody>();
            }

            return collision;
        }

        private Vector2f GetWorldMin()
        {
            Vector2f pos = Transform.Position + Offset;
            return pos - Size / 2;
        }

        private Vector2f GetWorldMax()
        {
            Vector2f pos = Transform.Position + Offset;
            return pos + Size / 2;
        }
    }
}
