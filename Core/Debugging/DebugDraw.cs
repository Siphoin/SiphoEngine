using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SiphoEngine.Core.Debugging
{
    public static class DebugDraw
    {
        private static readonly List<Drawable> _shapes = new List<Drawable>();
        private static readonly List<Vertex[]> _lines = new List<Vertex[]>();

        // Примитивы для переиспользования
        private static readonly RectangleShape _sharedRect = new RectangleShape();
        private static readonly CircleShape _sharedCircle = new CircleShape(1f, 32);
        private static readonly Vertex[] _lineBuffer = new Vertex[2];

        public static void DrawBox(Vector2f position, Vector2f size, Color color, float thickness = 1f)
        {
            _sharedRect.Size = size;
            _sharedRect.Position = position - size / 2;
            _sharedRect.FillColor = Color.Transparent;
            _sharedRect.OutlineColor = color;
            _sharedRect.OutlineThickness = thickness;
            _shapes.Add(new RectangleShape(_sharedRect));
        }

        public static void DrawCircle(Vector2f center, float radius, Color color, float thickness = 1f)
        {
            _sharedCircle.Radius = radius;
            _sharedCircle.Position = center - new Vector2f(radius, radius);
            _sharedCircle.FillColor = Color.Transparent;
            _sharedCircle.OutlineColor = color;
            _sharedCircle.OutlineThickness = thickness;
            _shapes.Add(new CircleShape(_sharedCircle));
        }

        public static void DrawLine(Vector2f start, Vector2f end, Color color)
        {
            _lineBuffer[0] = new Vertex(start, color);
            _lineBuffer[1] = new Vertex(end, color);
            _lines.Add(new Vertex[] { _lineBuffer[0], _lineBuffer[1] });
        }

        public static void Render(RenderTarget target)
        {
            // Рисуем все фигуры
            foreach (var shape in _shapes)
            {
                target.Draw(shape);
            }

            // Рисуем все линии
            foreach (var line in _lines)
            {
                target.Draw(line, 0, 2, PrimitiveType.Lines);
            }

            Clear();
        }

        public static void Clear()
        {
            _shapes.Clear();
            _lines.Clear();
        }
    }
}