using SFML.Graphics;
using SiphoEngine.Core;
using SiphoEngine.Core.PlayerLoop;

namespace SiphoEngine.UI
{
    public abstract class UIComponent : Component, IAwakable
    {
        public event System.Action OnVisualChanged;

        public RectTransform RectTransform { get; private set; }

        public virtual void Awake()
        {
            RectTransform = GameObject.AddComponent<RectTransform>();
            if (GameObject.GetComponentInParent<Canvas>() is Canvas canvas)
            {
                canvas.RegisterUIComponent(this);
            }
        }

        protected void MarkVisualChanged()
        {
            OnVisualChanged?.Invoke();
        }

        public abstract void Draw(RenderTarget target);

        public override void Dispose()
        {
            if (GameObject.GetComponentInParent<Canvas>() is Canvas canvas)
            {
                canvas.UnregisterUIComponent(this);
            }
            base.Dispose();
        }
    }
}