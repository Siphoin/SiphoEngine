using System;
using System.Text.Json.Serialization;
namespace SiphoEngine.Core
{
    [Serializable]
    public abstract class Component : Object, IDisposable
    {
        [JsonIgnore]
        public GameObject? GameObject { get; internal set; }
        [JsonIgnore]
        public Transform Transform => GameObject?.Transform;

        public override void Destroy()
        {
            GameObject?.Scene?.UnregisterComponent(this);
            GameObject?.Destroy();
        }

        public void Dispose()
        {
            GameObject?.Scene?.UnregisterComponent(this);
            GameObject = null;
        }
    }
}
