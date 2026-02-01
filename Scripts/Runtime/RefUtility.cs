using System;
using UnityEngine;

namespace PostEnot.Toolkits
{
    public static class RefUtility
    {
        public static Action<MonoBehaviour> InitReferencesImplementation { get; set; }

        public static void InitReferences(MonoBehaviour monoBehaviour) => InitReferencesImplementation?.Invoke(monoBehaviour);
    }
}
