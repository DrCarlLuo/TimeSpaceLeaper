using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.ManagedTime
{
    public static class ManagedTimeUtilities
    {
        public static void ManagedDestroy(this UnityEngine.Object obj, MonoBehaviour removeObj, ITimeManaged timeManaged)
        {
            if (timeManaged != null)
                TimeStateManager.Instance.Unregister(timeManaged);
            
            UnityEngine.Object.Destroy(removeObj);
        }

        public static void ManagedDestroy(this UnityEngine.Object obj, MonoBehaviour removeObj, ITimeManaged timeManaged, float t)
        {
            if (timeManaged != null)
                TimeStateManager.Instance.Unregister(timeManaged);
            UnityEngine.Object.Destroy(removeObj,t);
        }

        public static void RealDestroyGO(GameObject go)
        {
            if (go == null) return;
            var comps = go.GetComponentsInChildren<ITimeManaged>();
            foreach (var c in comps)
                TimeStateManager.Instance.Unregister(c);
            UnityEngine.Object.Destroy(go);
        }

        public static void ManagedDestroy(this UnityEngine.Object obj, GameObject removeObj)
        {
            if (removeObj == null) return;
            var mgo = removeObj.GetComponent<ManagedGameObject>();
            if (mgo != null)
                mgo.FakeDestroy();
            else
                RealDestroyGO(removeObj);
        }

        public static void ManagedDestroy(this UnityEngine.Object obj, GameObject removeObj, float t)
        {
            ManagedScheduler.Instance.Schedule(t, () => ManagedDestroy(null, removeObj));
        }

        public static void RegisterEntireGO(GameObject go)
        {
            var comps = go.GetComponentsInChildren<ITimeManaged>();
            foreach (var c in comps)
                TimeStateManager.Instance.Register(c);
        }
    }
}
