using System;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.ManagedTime
{
    /// <summary>
    /// A component that record the data of the gameobject itself
    /// </summary>
    public class ManagedGameObject : MonoBehaviour, ITimeManaged
    {
        private TimeStateContainer timeStateContainer = new TimeStateContainer();
        public bool RecordTransform = false;
        public bool RecordRigidBody = false;
        public bool FakeDestroyed = false;

        Rigidbody2D rb = null;

        void Awake()
        {
            NodeInMgr = null;
            if (RecordRigidBody)
                rb = GetComponent<Rigidbody2D>();
        }

        void OnDestroy()
        {
            if(TimeStateManager.Instance != null)
                TimeStateManager.Instance.Unregister(this);
        }

        public void FakeDestroy()
        {
            FakeDestroyed = true;
            gameObject.SetActive(false);
            GameObject go = gameObject; //
            ManagedScheduler.Instance.Schedule(TimeStateManager.MaxRecordStatesCnt + 1, () => {
                ManagedTimeUtilities.RealDestroyGO(go);
            });
        }

        public TimeStateContainer GetTimeStateContainer => timeStateContainer;

        public LinkedListNode<ITimeManaged> NodeInMgr { get; set; }

        public void ExposeTimeData()
        {
            bool newActive = gameObject.activeSelf;
            TimeScribe.Look_Value(ref newActive);
            if (newActive != gameObject.activeSelf)
                gameObject.SetActive(newActive);
            if(RecordTransform)
                TimeScribe.Look_Transform(transform);
            if(RecordRigidBody)
            {
                rb.velocity = TimeScribe.Look_Property(rb.velocity);
            }
            TimeScribe.Look_Value(ref FakeDestroyed);
        }

        public void RewindToPreBirth()
        {
            if (TimeStateManager.Instance.IsPreview)
                gameObject.SetActive(false);
            else
                ManagedTimeUtilities.RealDestroyGO(gameObject);
        }
    }


}
