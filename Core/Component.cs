using System;
using System.Text.Json.Serialization;
using SiphoEngine.Core.Coroutines;
using SiphoEngine.Core.SiphoEngine;
namespace SiphoEngine.Core
{
    [Serializable]
    public abstract class Component : Object, IDisposable
    {
        [JsonIgnore]
        public GameObject? GameObject { get; internal set; }
        [JsonIgnore]
        public Transform Transform => GameObject?.Transform;

        public object AddComponent(Type type)
        {
            return GameObject?.AddComponent(type);
        }

        public T AddComponent<T>() where T : Component, new()
        {
            return GameObject?.AddComponent<T>();
        }

        public T? GetComponent<T>() where T : Component
        {
            return GameObject?.GetComponent<T>();
        }


        public AsyncCoroutine StartCoroutine(IEnumerator<ICoroutineYield> coroutine)
        {
            return GameObject.StartCoroutine(coroutine);
        }

        public void StopCoroutine(ref AsyncCoroutine coroutine)
        {
            GameObject?.StopCoroutine( ref coroutine);
        }

        public void DelayAction(float delay, Action action)
        {
           GameObject? .DelayAction(delay, action);
        }

        public void StopAllCoroutines()
        {
           GameObject?.StopAllCoroutines();
        }

        public override void Destroy()
        {
            GameObject?.Scene?.UnregisterComponent(this);
            GameObject?.Destroy();
        }

        public virtual void Dispose()
        {
            GameObject?.Scene?.UnregisterComponent(this);
            GameObject = null;
        }
    }
}
