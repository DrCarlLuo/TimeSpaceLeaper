using System;
using UnityEngine;
using System.Collections.Generic;

namespace LAB.ManagedTime
{
    public class TimeStateManager : MonoBehaviour
    {
        #region Singleton
        public static TimeStateManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
                Log.Error("Multiple Time Manager!");
            Instance = this;
        }
        #endregion

        public int CurrentTimeStamp { get; private set; }
        public List<float> TimeStampTime { get; private set; }
        public float FakeTime { get; private set; }

        public bool IsPreview { get; private set; }
        public int PreviewStamp { get; private set; }
        public float PreviewTime => TimeStampTime[PreviewStamp];


        private LinkedList<ITimeManaged> Rewindables = new LinkedList<ITimeManaged>();
        private bool started;
        private float remainingUpdateTime;
        private int HasStashedState;

        public static readonly float RecordInterval = 0.05f;
        public static readonly int MaxRecordStatesCnt = 200;
        public int EarliestExistingTimeState => Math.Max(CurrentTimeStamp - MaxRecordStatesCnt + 1 - HasStashedState, 1);
        public float EarliestExistingFakeTime => TimeStampTime[EarliestExistingTimeState];
        public int MaxRewindableTimeStamp => Math.Min(MaxRecordStatesCnt + HasStashedState, CurrentTimeStamp) - 1;
        public float MaxRewindableTime => RecordInterval * MaxRewindableTimeStamp;

        private void Init()
        {
            started = true;
            CurrentTimeStamp = 0;
            TimeStampTime = new List<float>();
            TimeStampTime.Add(0f);
            FakeTime = 0f;
            remainingUpdateTime = RecordInterval;
            HasStashedState = 0;
            foreach (var node in Rewindables)
                node.GetTimeStateContainer.Init();
            ManagedScheduler.Instance.Reset();
        }

        private void RecordAll(bool removeExceedingState = true)
        {
            ++CurrentTimeStamp;
            if (TimeStampTime.Count >= CurrentTimeStamp + 1)
                TimeStampTime[CurrentTimeStamp] = FakeTime;
            else
                TimeStampTime.Add(FakeTime);
            var listNode = Rewindables.First;
            while (listNode != null)
            {
                var node = listNode.Value;
                var curListNode = listNode;
                listNode = listNode.Next;
                if (node == null)
                {
                    Rewindables.Remove(curListNode);
                    continue;
                }
                TimeScribe.EnterNode(ScribeMode.Probe, null);
                node.ExposeTimeData();
                byte[] state = new byte[TimeScribe.ProbeRes];
                TimeScribe.EnterNode(ScribeMode.Record, state);
                node.ExposeTimeData();
                node.GetTimeStateContainer.AddTimeState(state, CurrentTimeStamp);
                if (removeExceedingState)
                    node.GetTimeStateContainer.RemoveStatesBefore(EarliestExistingTimeState);
                TimeScribe.ExitNode();
            }
        }

        private void GoBackTo(int timeStamp, bool isPreview, bool removeBackPoint = false)
        {
            IsPreview = isPreview;
            PreviewStamp = timeStamp;
            if (!isPreview)
            {
                CurrentTimeStamp = removeBackPoint ? timeStamp - 1 : timeStamp;
                FakeTime = TimeStampTime[CurrentTimeStamp];
            }
            var listNode = Rewindables.First;
            while(listNode!=null)
            {
                var node = listNode.Value;
                var curListNode = listNode;
                listNode = listNode.Next;
                if (node == null)
                {
                    Rewindables.Remove(curListNode);
                    continue;
                }
                var container = node.GetTimeStateContainer;
                if (container.TryGetStateAtTimeStamp(timeStamp, out var state))
                {
                    TimeScribe.EnterNode(ScribeMode.Rewind, state);
                    node.ExposeTimeData();
                    TimeScribe.ExitNode();
                }
                else
                    node.RewindToPreBirth();
                if (!isPreview)
                    container.RemoveStatesAfter(removeBackPoint ? timeStamp - 1 : timeStamp);
            }
        }

        void Update()
        {
            if (!started) return;
            remainingUpdateTime -= Time.deltaTime;
            FakeTime += Time.deltaTime;
            if (remainingUpdateTime < float.Epsilon)
            {
                RecordAll();
                remainingUpdateTime = RecordInterval;
                ManagedScheduler.Instance.UpdateStampAction();
            }
            if(CurrentTimeStamp >= 1)
                ManagedScheduler.Instance.UpdateTimeAction();
        }

        #region interfaces
        public void BeginRecord() => Init();

        public void EndRecord() => started = false;

        public void Register(ITimeManaged timeManaged)
        {
            timeManaged.NodeInMgr = Rewindables.AddLast(timeManaged);
        }

        public void Unregister(ITimeManaged timeManaged)
        {
            //Rewindables.Remove(timeManaged);
            if (timeManaged.NodeInMgr != null)
            {
                //Rewindables.Remove(timeManaged.NodeInMgr);
                timeManaged.NodeInMgr.Value = null;
                timeManaged.NodeInMgr = null;
            }
        }

        public void UnregisterAll()
        {
            Rewindables.Clear();
        }

        public void Rewind(int timeStampDiff, bool isPreview)
        {
            int timeStampThen = CurrentTimeStamp - timeStampDiff;
            if (timeStampThen < EarliestExistingTimeState)
            {
                Log.Warning("Try to rewind to a time that is too early");
                return;
            }
            GoBackTo(timeStampThen, isPreview);
            if(timeStampDiff > 0)
            {
                //DONT Update scheduler if re-rewind to "now", or some actions will be executed twice!
                ManagedScheduler.Instance.UpdateTimeAction();
                ManagedScheduler.Instance.UpdateStampAction();
            }
        }

        public void StashCurrentTime()
        {
            HasStashedState = 1;
            RecordAll(false);
        }

        public void BackToNow()
        {
            if (HasStashedState == 1)
            {
                HasStashedState = 0;
                GoBackTo(CurrentTimeStamp, false, true);
            }
            else
            {
                Log.Warning("No stashed current state");
            }
        }
        #endregion
    }
}
