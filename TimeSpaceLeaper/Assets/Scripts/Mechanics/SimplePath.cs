using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    public enum PathTypeEnum
    {
        BACK_FORTH,
        LOOP,
        ONCE
    }

    [Serializable]
    public class SimplePath : ITimeManagedSub
    {
        public PathTypeEnum PathType;
        public Transform[] Nodes;
        public float[] StageTime;
        public int CurPos { get; private set; }
        public Transform CurNode => Nodes[CurPos];
        public float CurStageTime => StageTime[CurPos];
        public Transform NxtNode => NxtPos == -1 ? null: Nodes[NxtPos];
        public int NodeCnt => Nodes.Length;
        bool isMovingBack;

        public void Init()
        {
            CurPos = 0;
            isMovingBack = false;
        }

        public int NxtPos
        {
            get
            {
                if (CurPos == Nodes.Length - 1)
                {
                    if (PathType == PathTypeEnum.BACK_FORTH)
                        isMovingBack = true;
                    else if (PathType == PathTypeEnum.LOOP)
                        return 0;
                    else
                        return -1;
                }
                if (CurPos == 0 && isMovingBack)
                    isMovingBack = false;
                return isMovingBack ? CurPos - 1 : CurPos + 1;
            }
        }

        public void MoveNext() => CurPos = NxtPos;
        public void ForceSetNode(int x) => CurPos = x;

        public void ExposeTimeData()
        {
            CurPos = TimeScribe.Look_Property(CurPos);
            TimeScribe.Look_Value(ref isMovingBack);
        }
    }
}
