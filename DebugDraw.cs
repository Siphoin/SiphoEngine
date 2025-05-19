
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SiphoEngine
{
    internal static class DebugDraw
    {
        private static List<RectangleShape> _boxColliders = new List<RectangleShape>();
        private static List<CircleShape> _circleColliders = new List<CircleShape>();

        internal static void DrawBoxCollider(Vector2f position, Vector2f size, Vector2f offset)
        {
            var rect = new RectangleShape(size)
            {
                Position = position + offset - size / 2,
                FillColor = Color.Transparent,
                OutlineColor = Color.Green,
                OutlineThickness = 1f
            };
            _boxColliders.Add(rect);
        }

        internal static void DrawCircleCollider(Vector2f position, float radius, Vector2f offset)
        {
            var circle = new CircleShape(radius)
            {
                Position = position + offset - new Vector2f(radius, radius),
                FillColor = Color.Transparent,
                OutlineColor = Color.Green,
                OutlineThickness = 1f
            };
            _circleColliders.Add(circle);
        }

        internal static void Render(RenderTarget target)
        {
            foreach (var box in _boxColliders)
            {
                target.Draw(box);
            }

            foreach (var circle in _circleColliders)
            {
                target.Draw(circle);
            }

            Clear();
        }

        private static void Clear()
        {
            _boxColliders.Clear();
            _circleColliders.Clear();
        }
    }
}