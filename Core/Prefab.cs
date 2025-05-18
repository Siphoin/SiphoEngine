using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFML.System;

namespace SiphoEngine.Core
{
    public static class Prefab
    {
        private const string PrefabsDir = "Resources/Prefabs/";
        private static Dictionary<string, PrefabData> _prefabs = new Dictionary<string, PrefabData>();

        [Serializable]
        private class PrefabData
        {
            public Vector2f Position { get; set; }
            public float Rotation { get; set; }
            public Vector2f Scale { get; set; } = new Vector2f(1, 1);
            public List<ComponentData> Components { get; set; } = new List<ComponentData>();
        }

        [Serializable]
        private class ComponentData
        {
            public string TypeName { get; set; }
            public Dictionary<string, string> SerializedProperties { get; set; } = new Dictionary<string, string>();
        }

        private class ShouldSerializeContractResolver : DefaultContractResolver
        {
            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var members = base.GetSerializableMembers(objectType);
                return members.Where(m =>
                    !m.GetCustomAttributes(typeof(NonSerializedAttribute), true).Any()
                ).ToList();
            }
        }

        static Prefab()
        {
            if (!Directory.Exists(PrefabsDir))
                Directory.CreateDirectory(PrefabsDir);
        }

        public static void CreatePrefab(string prefabName, GameObject gameObject)
        {
            string path = Path.Combine(PrefabsDir, $"{prefabName}.prefab");

            // В режиме Release проверяем существование префаба
            if (File.Exists(path))
            {
                return; // Префаб уже существует, ничего не делаем
            }

            var prefabData = new PrefabData
            {
                Position = gameObject.Transform.Position,
                Rotation = gameObject.Transform.Rotation,
                Scale = gameObject.Transform.Scale
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ShouldSerializeContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            foreach (var component in gameObject.Components)
            {
                if (component is Transform) continue;

                var componentType = component.GetType();

                var componentData = new ComponentData
                {
                    TypeName = componentType.AssemblyQualifiedName
                };

                foreach (var prop in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        var value = prop.GetValue(component);
                        if (value is GameObject || value is Transform)
                        {
                            continue;
                        }
                        componentData.SerializedProperties[prop.Name] =
                            JsonConvert.SerializeObject(value, settings);
                    }
                    catch { /* Игнорируем несериализуемые свойства */ }
                }

                foreach (var field in componentType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        var value = field.GetValue(component);
                        componentData.SerializedProperties[field.Name] =
                            JsonConvert.SerializeObject(value, settings);
                    }
                    catch { /* Игнорируем несериализуемые поля */ }
                }

                prefabData.Components.Add(componentData);
            }

            SavePrefab(prefabName, prefabData);
        }


        private static void SavePrefab(string prefabName, PrefabData data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            string path = Path.Combine(PrefabsDir, $"{prefabName}.prefab");

#if !DEBUG
            byte[] encrypted = SimpleEncrypt(json);
            File.WriteAllBytes(path, encrypted);
#else
            File.WriteAllText(path, json);
#endif
        }

        private static PrefabData LoadPrefab(string prefabName)
        {
            string path = Path.Combine(PrefabsDir, $"{prefabName}.prefab");

#if !DEBUG
            byte[] encrypted = File.ReadAllBytes(path);
            string json = SimpleDecrypt(encrypted);
#else
            string json = File.ReadAllText(path);
#endif

            return JsonConvert.DeserializeObject<PrefabData>(json);
        }

        public static GameObject Instantiate(string prefabName, string name = null)
        {
#if !DEBUG
            // В режиме Release всегда загружаем из файла
            var prefabData = LoadPrefab(prefabName);
#else
    if (!_prefabs.TryGetValue(prefabName, out var prefabData))
    {
        prefabData = LoadPrefab(prefabName);
        _prefabs[prefabName] = prefabData;
    }
#endif

            var go = new GameObject(name ?? prefabName)
            {
                Transform =
        {
            Position = prefabData.Position,
            Rotation = prefabData.Rotation,
            Scale = prefabData.Scale
        }
            };


            foreach (var componentData in prefabData.Components)
            {
                Type componentType = Type.GetType(componentData.TypeName);
                if (componentType == null) continue;

                var component = go.AddComponent(componentType);

                foreach (var prop in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (componentData.SerializedProperties.TryGetValue(prop.Name, out var jsonValue))
                    {
                        try
                        {
                            var value = JsonConvert.DeserializeObject(jsonValue, prop.PropertyType);
                            prop.SetValue(component, value);
                        }
                        catch { /* Пропускаем ошибки десериализации */ }
                    }
                }

                foreach (var field in componentType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (componentData.SerializedProperties.TryGetValue(field.Name, out var jsonValue))
                    {
                        try
                        {
                            var value = JsonConvert.DeserializeObject(jsonValue, field.FieldType);
                            field.SetValue(component, value);
                        }
                        catch { /* Пропускаем ошибки десериализации */ }
                    }
                }
            }
            GameEngine.ActiveScene?.AddGameObject(go);
            return go;
        }
#if !DEBUG
        private static byte[] SimpleEncrypt(string data)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)(bytes[i] ^ 0x55);
            return bytes;
        }

        private static string SimpleDecrypt(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ 0x55);
            return System.Text.Encoding.UTF8.GetString(data);
        }
#endif
    }
}