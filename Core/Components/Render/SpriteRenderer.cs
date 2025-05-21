using SFML.Graphics;
using SFML.System;
using SiphoEngine.Core;
using SiphoEngine.Core.PlayerLoop;

namespace SiphoEngine.Components
{
    public class SpriteRenderer : Component, IDrawable, IAwakable
    {
        private Sprite _sprite;
        private RectangleShape _fallbackShape;
        private bool _hasSprite = false;

        private static readonly Color FallbackColor = new Color(255, 105, 180);

        public Color Color { get; set; } = Color.White;
        public Vector2f Size { get; set; } = new Vector2f(50, 50);

        public Texture Texture
        {
            get => _sprite?.Texture;
            set
            {
                if (value != null)
                {
                    _sprite = new Sprite(value)
                    {
                        Color = Color // Применяем текущий цвет
                    };
                    _hasSprite = true;
                }
                else
                {
                    _hasSprite = false;
                }
            }
        }

        public void Awake()
        {
            _fallbackShape = new RectangleShape(Size)
            {
                FillColor = FallbackColor,
                Origin = new Vector2f(Size.X / 2, Size.Y / 2)
            };
        }

        public void Draw(RenderTarget target)
        {
            if (_hasSprite && _sprite != null)
            {
                _sprite.Position = Transform.WorldPosition;
                _sprite.Rotation = Transform.Rotation;
                _sprite.Scale = Transform.Scale;
                _sprite.Color = Color;

                target.Draw(_sprite);
            }
            else
            {
                _fallbackShape.Position = Transform.WorldPosition;
                _fallbackShape.Rotation = Transform.Rotation;
                _fallbackShape.Scale = Transform.Scale;
                target.Draw(_fallbackShape);
            }
        }

        public override void Dispose()
        {
            _fallbackShape?.Dispose();
            _sprite?.Dispose();
            base.Dispose();
        }
    }
}