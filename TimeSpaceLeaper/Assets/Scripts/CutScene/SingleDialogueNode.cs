using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAB.AI;
using LAB.ManagedTime;
using LAB.Mechanics;

namespace LAB.CutScene
{
    class SingleDialogueNode
    {
        public string Context;
        public bool PauseCutScene = false;
        public bool ResumeCutSceneAfter = false;
        public SingleDialogueNode NxtNode = null;
        public Action NodeAction = null;
        public Action NodeActionAfter = null;
        public bool FreezeTime = false;
        public bool TimeReflow = false;
        public bool KeyboardContinue = true;
    }
}
