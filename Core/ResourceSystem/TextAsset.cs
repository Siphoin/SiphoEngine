namespace SiphoEngine.Core.ResourceSystem
{
    public struct TextAsset
    {
        public string Text {  get; set; }

        internal TextAsset (string text)
        {
            Text = text;
        }
    }
}
