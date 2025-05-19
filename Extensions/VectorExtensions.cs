using SFML.System;

namespace SiphoEngine.MathExtensions
{
    public static class VectorExtensions
    {
        public static Vector2f Normalized(this Vector2f vector)
        {
            float length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (length > 0)
                return new Vector2f(vector.X / length, vector.Y / length);
            return vector;
        }

        public static bool IsZero (this Vector2f vector)
        {
            return vector.X == 0 && vector.Y == 0;
        }

        public static void Normalize(this ref Vector2f vector)
        {
            float length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (length > 0)
            {
                vector.X /= length;
                vector.Y /= length;
            }
        }

        public static float Magnitude(this Vector2f vector)
        {
            return vector.Length();
        }

        public static float Length(this Vector2f vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        public static float Dot(this Vector2f a, Vector2f b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static float Distance(this Vector2f a, Vector2f b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}