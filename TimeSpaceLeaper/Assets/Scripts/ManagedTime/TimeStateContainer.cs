using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAB.Utility;

namespace LAB.ManagedTime
{
    /// <summary>
    /// A container for storing and managing time states of an individual class
    /// </summary>
    public class TimeStateContainer
    {
        /*
         * Caution:
         * When go back to a certain time state, all the tailing time states should be destroyed in case there are dead references.
        */
        protected Deque<byte[]> StateQue;
        public int Length => StateQue.Count;
        public int EarliestTimeStamp { get; protected set; }
        public int LatestTimeStamp { get; protected set; }

        public TimeStateContainer()
        {
            Init();
        }

        public void Init()
        {
            StateQue = new Deque<byte[]>();
            EarliestTimeStamp = LatestTimeStamp = 0;
        }

        public void CloneFrom(TimeStateContainer other)
        {
            EarliestTimeStamp = other.EarliestTimeStamp;
            LatestTimeStamp = other.LatestTimeStamp;
            StateQue.Clear();
            foreach(var state in other.StateQue)
            {
                StateQue.AddToBack((byte[])state.Clone());
            }
        }

        public bool TryGetStateAtTimeStamp(int timeStamp, out byte[] res)
        {
            res = null;
            if (StateQue.Count <= 0)
                return false;
            if (timeStamp > LatestTimeStamp || timeStamp < EarliestTimeStamp)
                return false;
            int pos = timeStamp - EarliestTimeStamp;
            if (pos >= StateQue.Count)
            {
                Log.Error("A timestatecontainer contains less elements than it should have. This may be caused by a non-continuous container");
                return false;
            }
            res = StateQue.ElementAt(pos);
            return true;
        }

        public void AddTimeState(byte[] state, int timeStamp)
        {
            if(StateQue.Count == 0)
            {
                EarliestTimeStamp = LatestTimeStamp = timeStamp;
                StateQue.AddToBack(state);
                return;
            }
            if (LatestTimeStamp >= timeStamp)
                Log.Warning("Time state in later timestamp exists which should be destroyed before.");
            if (LatestTimeStamp != timeStamp - 1)
                Log.Error("A time container is not continuous!");
            LatestTimeStamp = timeStamp;
            StateQue.AddToBack(state);
        }

        public void RemoveStatesBefore(int timeStamp)
        {
            while(EarliestTimeStamp < timeStamp && StateQue.Count > 0)
            {
                ++EarliestTimeStamp;
                StateQue.RemoveFromFront();
            }
        }

        public void RemoveStatesAfter(int timeStamp)
        {
            while(LatestTimeStamp > timeStamp && StateQue.Count > 0)
            {
                --LatestTimeStamp;
                StateQue.RemoveFromBack();
            }
        }
    }

}
