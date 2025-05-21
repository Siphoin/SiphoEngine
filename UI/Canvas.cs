using SFML.Graphics;
using SFML.System;
using SiphoEngine.Core.PlayerLoop;
using SiphoEngine.Core;
using System.Collections.Generic;

namespace SiphoEngine.UI
{
    public sealed class Canvas : Component, IDrawable, IAwakable
    {
        private RenderTexture _renderTexture;
        private Sprite _canvasSprite;
        private bool _needsRedraw = true;
        private HashSet<UIComponent> _uiComponents = new HashSet<UIComponent>();

        public Vector2f Size { get; private set; }
        public Color BackgroundColor { get; set; } = new Color(0, 0, 0, 0);

        public bool ActiveSelf => GameObject ? GameObject.ActiveSelf : false;

        public void Awake()
        {
            Initialize(new Vector2f(800, 600));
            RegisterExistingUIComponents();
        }

        private void RegisterExistingUIComponents()
        {
            foreach (var uiComponent in GameObject.GetComponentsInChildren<UIComponent>())
            {
                RegisterUIComponent(uiComponent);
            }
        }

        public void Initialize(Vector2f size)
        {
            Size = size;
            _renderTexture = new RenderTexture((uint)size.X, (uint)size.Y);
            _canvasSprite = new Sprite(_renderTexture.Texture);
        }

        internal void RegisterUIComponent(UIComponent component)
        {
            if (_uiComponents.Add(component))
            {
                component.OnVisualChanged += MarkDirty;
                component.AddComponent<RectTransform>();
                MarkDirty();
            }
        }

        internal void UnregisterUIComponent(UIComponent component)
        {
            if (_uiComponents.Remove(component))
            {
                component.OnVisualChanged -= MarkDirty;
                MarkDirty();
            }
        }

        internal void MarkDirty()
        {
            _needsRedraw = true;
        }

        public void Draw(RenderTarget target)
        {
            if (_needsRedraw)
            {
                RedrawCanvas();
                _needsRedraw = false;
            }

            _canvasSprite.Position = Transform.WorldPosition;
            target.Draw(_canvasSprite);
        }

        private void RedrawCanvas()
        {
            _renderTexture.Clear(BackgroundColor);

            foreach (var uiComponent in _uiComponents)
            {
                if (uiComponent.GameObject.ActiveSelf)
                {
                    uiComponent.Draw(_renderTexture);
                }
            }

            _renderTexture.Display();
        }

        public override void Dispose()
        {
            foreach (var uiComponent in _uiComponents)
            {
                uiComponent.OnVisualChanged -= MarkDirty;
            }
            _uiComponents.Clear();

            _renderTexture?.Dispose();
            _canvasSprite?.Dispose();
            base.Dispose();
        }
    }
}