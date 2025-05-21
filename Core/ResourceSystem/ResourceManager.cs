using SFML.Graphics;
using SiphoEngine.Core.Debugging;

namespace SiphoEngine.Core.ResourceSystem
{
    public class ResourceManager
    {
        private readonly string _contentRoot;
        private readonly Dictionary<string, Texture> _textures = new();
        private readonly Dictionary<string, Font> _fonts = new();
        private readonly Dictionary<string, TextAsset> _textAssets = new();

        private const string DEFAULT_PATH_ASSETS = "Content";

        public static ResourceManager Instance { get; private set; }

        internal ResourceManager(string contentFolder = "Content")
        {
            _contentRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, contentFolder);

            if (!Directory.Exists(_contentRoot))
            {
                Directory.CreateDirectory(_contentRoot);
            }

            Instance = this;
        }

        internal ResourceManager ()
        {
            _contentRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULT_PATH_ASSETS);
            if (!Directory.Exists(_contentRoot))
            {
                Directory.CreateDirectory(_contentRoot);
            }

            Instance = this;
        }

        public T LoadAsset<T>(string path)
        {
            string fullPath = GetFullResourcePath(path);
            Type type = typeof(T);
            object obj = null;

            if (type == typeof(Texture))
            {
                obj = LoadTexture(fullPath);
            }
            else if (type == typeof(Font))
            {
                obj = LoadFont(fullPath);
            }
            else if (type == typeof(TextAsset))
            {
                obj = LoadTextAsset(fullPath);
            }

            if (obj is null)
            {
                throw new ArgumentException($"Type {type.Name} is not a supported resource type or the file resource was not found");
            }

#if DEBUG
            Log($"Loaded asset: Type {type.Name} from {path}");
#endif

            return (T)obj;
        }

        private Texture LoadTexture(string path)
        {
            if (!_textures.TryGetValue(path, out Texture texture))
            {
                texture = new Texture(path);
                _textures.Add(path, texture);
            }
            return texture;
        }

        private Font LoadFont(string path)
        {
            if (!_fonts.TryGetValue(path, out Font font))
            {
                font = new Font(path);
                _fonts.Add(path, font);
            }
            return font;
        }

        private TextAsset LoadTextAsset(string path)
        {
            if (!_textAssets.TryGetValue(path, out TextAsset textAsset))
            {
                if (File.Exists(path))
                {
                    string context = File.ReadAllText(path);
                    textAsset = new TextAsset(context);
                    _textAssets.Add(path, textAsset);
                }
                else
                {
                    throw new FileNotFoundException($"File with path {path} not found");
                }
            }
            return textAsset;
        }

        public async Task<TextAsset> LoadTextAssetAsync(string path)
        {
            string fullPath = GetFullResourcePath(path);

            if (!_textAssets.TryGetValue(fullPath, out TextAsset textAsset))
            {
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"File with path {fullPath} not found");
                }

                using (StreamReader reader = new StreamReader(fullPath))
                {
                    string content = await reader.ReadToEndAsync();
                    textAsset = new TextAsset(content);
                    _textAssets.Add(fullPath, textAsset);
                }
            }
            return textAsset;
        }

        private string GetFullResourcePath(string relativePath)
        {
            return Path.Combine(_contentRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
        }

#if DEBUG
        private void Log (object message)
        {
            Debug.Log($"{nameof(ResourceManager)}: {message}");
        }
#endif
    }
}