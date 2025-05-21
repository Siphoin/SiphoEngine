using SFML.Graphics;

namespace SiphoEngine.Core.PlayerLoop
{
    internal interface IDrawable
    {
        bool ActiveSelf { get; }
        void Draw(RenderTarget target);
    }
}
