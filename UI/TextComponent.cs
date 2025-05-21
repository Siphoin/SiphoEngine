using SFML.Graphics;

namespace SiphoEngine.UI
{
    public class TextComponent : UIComponent
    {
        private Text _text = new Text();
        private string _content = "New Text";
        private uint _fontSize = 16;
        private Color _color = Color.White;

        public string Content
        {
            get => _content;
            set { if (_content != value) { _content = value; MarkVisualChanged(); } }
        }

        public uint FontSize
        {
            get => _fontSize;
            set { if (_fontSize != value) { _fontSize = value; MarkVisualChanged(); } }
        }

        public Color Color
        {
            get => _color;
            set { if (_color != value) { _color = value; MarkVisualChanged(); } }
        }

        public Font Font
        {
            get => _text.Font;
            set { if (_text.Font != value) { _text.Font = value; MarkVisualChanged(); } }
        }

        public override void Draw(RenderTarget target)
        {
            if (Font == null) return;

            _text.DisplayedString = Content;
            _text.CharacterSize = FontSize;
            _text.FillColor = Color;
            _text.Position = RectTransform.WorldPosition;

            target.Draw(_text);
        }

        public override void Dispose()
        {
            _text?.Dispose();
            base.Dispose();
        }
    }
}