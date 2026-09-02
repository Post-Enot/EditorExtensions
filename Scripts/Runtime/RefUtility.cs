using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace PostEnot.Toolkits
{
    public static partial class RefUtility
    {
        [AutoStaticsCleanup] public static Action<MonoBehaviour> InitReferencesImplementation { get; set; }

        public static void InitReferences(MonoBehaviour monoBehaviour) => InitReferencesImplementation?.Invoke(monoBehaviour);
    }
}
