using System;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.ManagedTime
{
    public abstract class ManagedMonoBehaviour : MonoBehaviour, ITimeManaged
    {
        private TimeStateContainer timeStateContainer = new TimeStateContainer();
        public TimeStateContainer GetTimeStateContainer => timeStateContainer;
        public LinkedListNode<ITimeManaged> NodeInMgr { get; set; }

        public virtual void ExposeTimeData()
        {
            enabled = TimeScribe.Look_Property(enabled);
        }

        public virtual void RewindToPreBirth()
        {
            if (TimeStateManager.Instance.IsPreview)
                enabled = false;
            else
                this.ManagedDestroy(this, this);
            
        }

        protected virtual void OnDestroy()
        {
            if(TimeStateManager.Instance != null)
                TimeStateManager.Instance.Unregister(this);
        }
    }

}
