using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB.ManagedTime
{
    public interface ITimeManaged
    {
        /// <summary>
        /// Creates a corresponding time state class. Use this to avoid run-time dynamic instance creating by System.Activator to increase performance.
        /// </summary>
        void ExposeTimeData();
        void RewindToPreBirth();

        TimeStateContainer GetTimeStateContainer { get; }

        LinkedListNode<ITimeManaged> NodeInMgr { get; set; }
    }
}
