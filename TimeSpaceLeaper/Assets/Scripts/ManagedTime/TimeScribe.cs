using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LAB.Utility;

namespace LAB.ManagedTime
{
    public static class TimeScribe
    {
        public static ScribeMode Mode = ScribeMode.None;
        public static byte[] CurNode { get; private set; }
        private static int NodePointer;
        public static int ProbeRes { get; private set; }

        public static void EnterNode(ScribeMode mode, byte[] NewNode)
        {
            Mode = mode;
            CurNode = NewNode;
            NodePointer = 0;
            ProbeRes = 0;
        }

        public static void ExitNode()
        {
            Mode = ScribeMode.None;
            CurNode = null;
        }

        public unsafe static void Look_Value<T>(ref T data) where T : unmanaged
        {
            if (Mode == ScribeMode.Record)
            {
                fixed(byte* dest = &CurNode[NodePointer])
                {
                    T* typedDest = (T*)dest;
                    *typedDest = data;
                }
                NodePointer += sizeof(T);
            }
            else if (Mode == ScribeMode.Rewind)
            {
                fixed(byte* src = &CurNode[NodePointer])
                {
                    T* typedSrc = (T*)src;
                    data = *typedSrc;
                }
                NodePointer += sizeof(T);
            }
            else if(Mode == ScribeMode.Probe)
            {
                ProbeRes += sizeof(T);
            }
            else
                Log.Error("Entering Undefined Time Scribe Mode");
            
        }

        public unsafe static void Look_Array<T>(ref T[] data, int orgLength = -1) where T : unmanaged
        {
            int length = orgLength;
            if (data != null)
                length = data.Length;
            Look_Value(ref length);
            if (length < 0)
                Log.Error("Fail to reload array time state!");
            if (Mode == ScribeMode.Record)
            {
                if (length == 0) return;
                fixed(T* src = data)
                fixed(byte* dest = &CurNode[NodePointer])
                {
                    T* typedDest = (T*)dest;
                    for (int i = 0; i < length; ++i)
                        typedDest[i] = src[i];
                }
                NodePointer += length * sizeof(T);
            }
            else if (Mode == ScribeMode.Rewind)
            {
                if(length == 0)
                {
                    data = new T[0];
                    return;
                }
                data = new T[length];
                fixed(T* dest = data)
                fixed(byte* src = &CurNode[NodePointer])
                {
                    T* typedSrc = (T*)src;
                    for (int i = 0; i < length; ++i)
                        dest[i] = typedSrc[i];
                }
                NodePointer += length * sizeof(T);
            }
            else if(Mode == ScribeMode.Probe)
            {
                ProbeRes += length * sizeof(T);
            }
            else
                Log.Error("Entering Undefined Time Scribe Mode");
        }

        public static T Look_Property<T>(T prop) where T:unmanaged
        {
            T tmp = prop;
            Look_Value(ref tmp);
            return tmp;
        }

        public static void Look_Sub(ITimeManagedSub subNode)
        {
            subNode.ExposeTimeData();
        }

        public static void Look_Transform(Transform transform)
        {
            transform.position = Look_Property(transform.position);
            transform.rotation = Look_Property(transform.rotation);
            transform.localScale = Look_Property(transform.localScale);
        }
    }

    public enum ScribeMode
    {
        Record,
        Rewind,
        Probe,
        None
    }
}
