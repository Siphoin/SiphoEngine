using SFML.Graphics;
using SFML.System;
using Keyboard = SFML.Window.Keyboard;
using Mouse = SFML.Window.Mouse;

namespace SiphoEngine.Core
{
    public static class Input
    {
        private static HashSet<Keyboard.Key> _pressedKeys = new HashSet<Keyboard.Key>();
        private static HashSet<Keyboard.Key> _keysDownThisFrame = new HashSet<Keyboard.Key>();
        private static HashSet<Keyboard.Key> _keysUpThisFrame = new HashSet<Keyboard.Key>();


        private static HashSet<Mouse.Button> _pressedMouseButtons = new HashSet<Mouse.Button>();
        private static HashSet<Mouse.Button> _mouseButtonsDownThisFrame = new HashSet<Mouse.Button>();
        private static HashSet<Mouse.Button> _mouseButtonsUpThisFrame = new HashSet<Mouse.Button>();


        private static Vector2f _mousePosition;

        public static Vector2f MousePosition => _mousePosition;

        internal static void Initialize(RenderWindow window)
        {
            window.KeyPressed += (sender, e) =>
            {
                if (!_pressedKeys.Contains(e.Code))
                {
                    _pressedKeys.Add(e.Code);
                    _keysDownThisFrame.Add(e.Code);
                }
            };

            window.KeyReleased += (sender, e) =>
            {
                _pressedKeys.Remove(e.Code);
                _keysUpThisFrame.Add(e.Code);
            };

            window.MouseMoved += (sender, e) =>
            {
                _mousePosition = new Vector2f(e.X, e.Y);
            };

            window.MouseButtonPressed += (sender, e) =>
            {
                if (!_pressedMouseButtons.Contains(e.Button))
                {
                    _pressedMouseButtons.Add(e.Button);
                    _mouseButtonsDownThisFrame.Add(e.Button);
                }
            };

            window.MouseButtonReleased += (sender, e) =>
            {
                _pressedMouseButtons.Remove(e.Button);
                _mouseButtonsUpThisFrame.Add(e.Button);
            };
        }

        internal static void Update()
        {
            _keysDownThisFrame.Clear();
            _keysUpThisFrame.Clear();
            _mouseButtonsDownThisFrame.Clear();
            _mouseButtonsUpThisFrame.Clear();
        }

        public static bool GetKey(Keyboard.Key key) => _pressedKeys.Contains(key);
        public static bool GetKeyDown(Keyboard.Key key) => _keysDownThisFrame.Contains(key);
        public static bool GetKeyUp(Keyboard.Key key) => _keysUpThisFrame.Contains(key);

        public static bool GetMouseButton(Mouse.Button button) => _pressedMouseButtons.Contains(button);
        public static bool GetMouseButtonDown(Mouse.Button button) => _mouseButtonsDownThisFrame.Contains(button);
        public static bool GetMouseButtonUp(Mouse.Button button) => _mouseButtonsUpThisFrame.Contains(button);
    }
}