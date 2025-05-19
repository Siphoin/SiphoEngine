
using SFML.Graphics;
using SFML.System;

namespace SiphoEngine.Core.Debugging
{
    internal static class DebugDraw
    {
        private static readonly RectangleShape _boxShape = new RectangleShape();
        private static readonly CircleShape _circleShape = new CircleShape(0);
        private static readonly List<Vertex[]> _lines = new List<Vertex[]>();

        internal static void DrawBoxCollider(Vector2f position, Vector2f size, Vector2f offset)
        {
            // Используем один объект для всех прямоугольников
            _boxShape.Size = size;
            _boxShape.Position = position + offset - size / 2;
            _boxShape.FillColor = Color.Transparent;
            _boxShape.OutlineColor = Color.Green;
            _boxShape.OutlineThickness = 1f;
            GameEngine.MainWindow?.Draw(_boxShape);
        }

        internal static void DrawCircleCollider(Vector2f position, float radius, Vector2f offset)
        {
            _circleShape.Radius = radius;
            _circleShape.Position = position + offset - new Vector2f(radius, radius);
            _circleShape.FillColor = Color.Transparent;
            _circleShape.OutlineColor = Color.Green;
            _circleShape.OutlineThickness = 1f;

            GameEngine.MainWindow?.Draw(_circleShape);
        }


        internal static void Render(RenderTarget target)
        {
            foreach (var line in _lines)
            {
                target.Draw(line, PrimitiveType.Lines);
            }
            _lines.Clear();
        }

        internal static void Clear()
        {
            _lines.Clear();
        }
    }
}