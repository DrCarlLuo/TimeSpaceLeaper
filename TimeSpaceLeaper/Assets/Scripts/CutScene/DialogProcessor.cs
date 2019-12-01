using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Mechanics;
using LAB.UI;
using LAB.Engine;
using UnityEngine.Events;

namespace LAB.CutScene
{
    class DialogProcessor : MonoBehaviour
    {
        public static DialogProcessor Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        public UnityEvent TimeTutorialAction;
        public SingleDialogueNode CurNode = null;

        public void EnterNode(SingleDialogueNode node)
        {
            if(node == null)
            {
                DialogueHelper.Instance.EndDialog();
                return;
            }
            CurNode = node;
            if (node.PauseCutScene)
                DirectorHelper.Instance.PauseCurrentMovie();
            if (node.FreezeTime)
                Time.timeScale = 0f;
            DialogueHelper.Instance.ShowText(node.Context,CurNode.KeyboardContinue);
            node.NodeAction?.Invoke();
        }

        private void Update()
        {
            if(CurNode!=null && CurNode.KeyboardContinue)
            {
                if(InputHandler.DialogContinue.KeyDown)
                {
                    if (CurNode.ResumeCutSceneAfter)
                        DirectorHelper.Instance.ResumeCurrentMovie();
                    if (CurNode.TimeReflow)
                        Time.timeScale = 1f;
                    CurNode.NodeActionAfter?.Invoke();
                    EnterNode(CurNode.NxtNode);
                }
            }
        }
        
        //These are only for testing!
        public void DialogPreset_Intro()
        {
            SingleDialogueNode A = new SingleDialogueNode();
            A.PauseCutScene = true;
            A.Context = "......";

            SingleDialogueNode B = new SingleDialogueNode();
            B.Context = "\nI can't remember how I ended up in this place.";

            SingleDialogueNode C = new SingleDialogueNode();
            C.Context = "\nI need to find a way out.\n";
            C.ResumeCutSceneAfter = true;

            A.NxtNode = B;
            B.NxtNode = C;
            EnterNode(A);
        }

        public void DialogPreset_TimeExplain()
        {
            SingleDialogueNode A = new SingleDialogueNode();
            A.PauseCutScene = true;
            A.Context = "\nIt seems that I cannot turn on the switch and pass the door at the same time...";
            A.FreezeTime = true;

            SingleDialogueNode B = new SingleDialogueNode();
            B.Context = "\nI need someone to help me keep the switch on.";

            SingleDialogueNode C = new SingleDialogueNode();
            C.Context = "\nBut obviously there is nobody here except me...";

            SingleDialogueNode D = new SingleDialogueNode();
            D.Context = "Ha! I can let myself <color=cyan>IN THE PAST</color> to help me hold the switch!";

            SingleDialogueNode E = new SingleDialogueNode();
            E.Context = "Meh... If I were to say such words a few days ago, I must have been out of my mind...";

            SingleDialogueNode F = new SingleDialogueNode();
            F.Context = "But it just happened, a few days ago I got some kind of power...";

            SingleDialogueNode G = new SingleDialogueNode();
            G.Context = "I can view the entire world as a four-dimensional space and can easily enter a <color=yellow>parallel universe</color> that is <b>a few seconds earlier</b> than the current universe...";

            SingleDialogueNode H = new SingleDialogueNode();
            H.Context = "Then I can utilize my <b><color=yellow>former self</color></b> to help me out. So that I can achieve something impossible with myself alone.";

            SingleDialogueNode I = new SingleDialogueNode();
            I.Context = "[Follow the instruction below to go to the parallel universe where the former Alyssa stood on the switch (Between <color=red>1.1s</color> ago and <color=red>1.8s</color> ago) ]";
            I.ResumeCutSceneAfter = true;
            I.KeyboardContinue = false;
            I.NodeAction = () => {
                TimeTutorialAction.Invoke();
                InputHandler.EnableTimeManipulation(true);
            };

            A.NxtNode = B;
            B.NxtNode = C;
            C.NxtNode = D;
            D.NxtNode = E;
            E.NxtNode = F;
            F.NxtNode = G;
            G.NxtNode = H;
            H.NxtNode = I;
            EnterNode(A);
        }

        public void DialogPreset_Perish()
        {
            SingleDialogueNode A = new SingleDialogueNode();
            A.PauseCutScene = true;
            A.Context = "\nShe perished...";

            SingleDialogueNode B = new SingleDialogueNode();
            B.Context = "\nAlas... I know this will happen.";

            SingleDialogueNode C = new SingleDialogueNode();
            C.Context = "If there are too many \"ME\" in one world, there will be some kind of conflict among my power. The older me can live for no more than 6 seconds...";

            SingleDialogueNode D = new SingleDialogueNode();
            D.Context = "Also, because of this reason, when I'm trying to travel to another time space, I must make sure that there are no more than three of \"me\".";

            SingleDialogueNode E = new SingleDialogueNode();
            E.Context = "Is killing \"myself\" in another universe a kind of <b><color=red>MURDER</color></b>?\n Bah... I'm... sorry... but I really need to get out of here first.";
            E.ResumeCutSceneAfter = true;

            A.NxtNode = B;
            B.NxtNode = C;
            C.NxtNode = D;
            D.NxtNode = E;
            EnterNode(A);
        }
    }
}
