using System;
using UnityEngine;
using System.Collections.Generic;

namespace LAB.ManagedTime
{
    //TODO: use timewheel to improve performance
    public class ManagedScheduler
    {
        private static ManagedScheduler instance;
        public static ManagedScheduler Instance
        {
            get
            {
                if (instance == null)
                    instance = new ManagedScheduler();
                return instance;
            }
        }
        private ManagedScheduler() { }

        private LinkedList<ManagedAction> timeActionList;
        private LinkedList<ManagedStampAction> stampActionList;

        public void Reset()
        {
            timeActionList = new LinkedList<ManagedAction>();
            stampActionList = new LinkedList<ManagedStampAction>();
        }

        public void Schedule(float t, Action action)
        {
            var ma = new ManagedAction();
            ma.CreationFakeTime = TimeStateManager.Instance.FakeTime;
            ma.ExecutionFakeTime = ma.CreationFakeTime + t;
            ma.action = action;
            ma.Executed = false;
            timeActionList.AddLast(ma);
        }

        public void Schedule(int t, Action action)
        {
            var msa = new ManagedStampAction();
            msa.CreationStamp = TimeStateManager.Instance.CurrentTimeStamp;
            msa.ExecutionStamp = msa.CreationStamp + t;
            msa.action = action;
            stampActionList.AddLast(msa);
        }

        public void UpdateTimeAction()
        {
            var node = timeActionList.First;
            while(node != null)
            {
                var ma = node.Value;
                var curNode = node;
                node = node.Next;
                if (ma.CreationFakeTime > TimeStateManager.Instance.FakeTime)
                    timeActionList.Remove(curNode);
                else
                {
                    if (ma.ExecutionFakeTime > TimeStateManager.Instance.FakeTime)
                        ma.Executed = false;
                    if(!ma.Executed && ma.ExecutionFakeTime <= TimeStateManager.Instance.FakeTime)
                    {
                        ma.Executed = true;
                        ma.action?.Invoke();
                    }
                    if(ma.ExecutionFakeTime < TimeStateManager.Instance.EarliestExistingFakeTime - 0.1f)
                        timeActionList.Remove(curNode);
                }
            }
        }

        public void UpdateStampAction()
        {
            var node = stampActionList.First;
            while(node != null)
            {
                var msa = node.Value;
                var curNode = node;
                node = node.Next;
                if (msa.CreationStamp > TimeStateManager.Instance.CurrentTimeStamp)
                    stampActionList.Remove(curNode);
                else
                {
                    if (msa.ExecutionStamp == TimeStateManager.Instance.CurrentTimeStamp)
                        msa.action?.Invoke();
                    if (msa.ExecutionStamp < TimeStateManager.Instance.EarliestExistingTimeState - 1)
                        stampActionList.Remove(curNode);
                }
            }
        }
    }

    class ManagedAction
    {
        public float CreationFakeTime;
        public float ExecutionFakeTime;
        public Action action;
        public bool Executed;
    }

    class ManagedStampAction
    {
        public int CreationStamp;
        public int ExecutionStamp;
        public Action action;
    }


}
