﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFML.System;
using SiphoEngine.Core.Debugging;
using SiphoEngine.Physics;

namespace SiphoEngine.Core
{
    public static class Prefab
    {
        private static readonly string PrefabsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Prefabs");
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
            try
            {
                if (!Directory.Exists(PrefabsDir))
                    Directory.CreateDirectory(PrefabsDir);
            }
            catch (Exception ex)
            {
                Debug.Error($"Failed to create prefabs directory: {ex}");
                throw;
            }
        }

        public static void CreatePrefab(string prefabName, GameObject gameObject)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
                throw new ArgumentException("Prefab name cannot be null or empty", nameof(prefabName));

            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            try
            {
                string path = GetPrefabPath(prefabName);

                if (File.Exists(path))
                {
                    Debug.Warn﻿($"Prefab '{prefabName}' already exists at path: {path}");
                    return;
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
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Error = (sender, args) => 
                    {
                        Debug.Error($"JSON serialization error: {args.ErrorContext.Error.Message}");
                        args.ErrorContext.Handled = true;
                    }
                };

                foreach (var component in gameObject.Components)
                {
                    if (component is Transform) continue;

                    var componentType = component.GetType();
                    var componentData = new ComponentData
                    {
                        TypeName = componentType.AssemblyQualifiedName
                    };

                    // Handle properties
                    foreach (var prop in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        try
                        {
                            if (!prop.CanRead) continue;
                            
                            var value = prop.GetValue(component);
                            if (value is GameObject || value is Transform || value is Collider)
                                continue;

                            componentData.SerializedProperties[prop.Name] = 
                                JsonConvert.SerializeObject(value, settings);
                        }
                        catch (Exception ex)
                        {
                            Debug.Warn﻿($"Failed to serialize property {prop.Name} of {componentType.Name}: {ex.Message}");
                        }
                    }

                    // Handle fields
                    foreach (var field in componentType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        try
                        {
                            var value = field.GetValue(component);
                            componentData.SerializedProperties[field.Name] = 
                                JsonConvert.SerializeObject(value, settings);
                        }
                        catch (Exception ex)
                        {
                            Debug.Warn﻿($"Failed to serialize field {field.Name} of {componentType.Name}: {ex.Message}");
                        }
                    }

                    prefabData.Components.Add(componentData);
                }

                SavePrefab(prefabName, prefabData);
            }
            catch (Exception ex)
            {
                Debug.Error($"Failed to create prefab '{prefabName}': {ex}");
                throw;
            }
        }

        private static string GetPrefabPath(string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
                throw new ArgumentException("Prefab name cannot be null or empty", nameof(prefabName));

            return Path.Combine(PrefabsDir, $"{prefabName}.prefab");
        }

        private static void SavePrefab(string prefabName, PrefabData data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string path = GetPrefabPath(prefabName);

#if !DEBUG
                byte[] encrypted = SimpleEncrypt(json);
                File.WriteAllBytes(path, encrypted);
#else
                File.WriteAllText(path, json);
                Debug.Log($"Prefab saved: {path}");
#endif
            }
            catch (Exception ex)
            {
                Debug.Error($"Failed to save prefab '{prefabName}': {ex}");
                throw;
            }
        }

        private static PrefabData LoadPrefab(string prefabName)
        {
            try
            {
                string path = GetPrefabPath(prefabName);

                if (!File.Exists(path))
                    throw new FileNotFoundException($"Prefab file not found: {path}");

#if !DEBUG
                byte[] encrypted = File.ReadAllBytes(path);
                string json = SimpleDecrypt(encrypted);
#else
                string json = File.ReadAllText(path);
#endif

                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) => 
                    {
                        Debug.Error($"JSON deserialization error: {args.ErrorContext.Error.Message}");
                        args.ErrorContext.Handled = true;
                    }
                };

                return JsonConvert.DeserializeObject<PrefabData>(json, settings);
            }
            catch (Exception ex)
            {
                Debug.Error($"Failed to load prefab '{prefabName}': {ex}");
                throw;
            }
        }

        public static GameObject Instantiate(string prefabName, GameObject sourceObject = null, string name = null)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
                throw new ArgumentException("Prefab name cannot be null or empty", nameof(prefabName));

            try
            {
                string path = GetPrefabPath(prefabName);
                PrefabData prefabData;

                if (!File.Exists(path))
                {
                    if (sourceObject == null)
                        throw new FileNotFoundException($"Prefab '{prefabName}' doesn't exist at path: {path} and no source object provided");

                    Debug.Log($"Creating new prefab from source object: {prefabName}");
                    CreatePrefab(prefabName, sourceObject);
                    prefabData = LoadPrefab(prefabName);
                }
                else
                {
#if !DEBUG
                    prefabData = LoadPrefab(prefabName);
#else
                    if (!_prefabs.TryGetValue(prefabName, out prefabData))
                    {
                        prefabData = LoadPrefab(prefabName);
                        _prefabs[prefabName] = prefabData;
                        Debug.Log($"Prefab cached: {prefabName}");
                    }
#endif
                }

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
                    try
                    {
                        Type componentType = Type.GetType(componentData.TypeName);
                        if (componentType == null)
                        {
                            Debug.Warn﻿($"Component type not found: {componentData.TypeName}");
                            continue;
                        }

                        var component = go.AddComponent(componentType);

                        // Set properties
                        foreach (var prop in componentType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (componentData.SerializedProperties.TryGetValue(prop.Name, out var jsonValue) && prop.CanWrite)
                            {
                                try
                                {
                                    var value = JsonConvert.DeserializeObject(jsonValue, prop.PropertyType);
                                    prop.SetValue(component, value);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Warn﻿($"Failed to set property {prop.Name} on {componentType.Name}: {ex.Message}");
                                }
                            }
                        }

                        // Set fields
                        foreach (var field in componentType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (componentData.SerializedProperties.TryGetValue(field.Name, out var jsonValue))
                            {
                                try
                                {
                                    var value = JsonConvert.DeserializeObject(jsonValue, field.FieldType);
                                    field.SetValue(component, value);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Warn﻿($"Failed to set field {field.Name} on {componentType.Name}: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Error($"Failed to process component {componentData.TypeName}: {ex}");
                    }
                }

                GameEngine.ActiveScene?.AddGameObject(go);
                Debug.Log($"Prefab instantiated: {prefabName}");
                return go;
            }
            catch (Exception ex)
            {
                Debug.Error($"Failed to instantiate prefab '{prefabName}': {ex}");
                throw;
            }
        }

#if !DEBUG
        private static byte[] SimpleEncrypt(string data)
        {
            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] = (byte)(bytes[i] ^ 0x55);
                return bytes;
            }
            catch (Exception ex)
            {
                Debug.Error($"Encryption failed: {ex}");
                throw;
            }
        }

        private static string SimpleDecrypt(byte[] data)
        {
            try
            {
                for (int i = 0; i < data.Length; i++)
                    data[i] = (byte)(data[i] ^ 0x55);
                return System.Text.Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                Debug.Error($"Decryption failed: {ex}");
                throw;
            }
        }
#endif
    }
}