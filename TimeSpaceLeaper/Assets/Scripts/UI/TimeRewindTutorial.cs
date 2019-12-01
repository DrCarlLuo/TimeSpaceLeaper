
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAB.AI;
using LAB.ManagedTime;
using LAB.Mechanics;
using LAB.CutScene;
using LAB.Engine;

namespace LAB.UI
{
    class TimeRewindTutorial : MonoBehaviour
    {
        TimeSelectionHelper timeSelectionHelper;
        public GameObject ShiftGO;
        public GameObject ShiftLetgoGO;
        public GameObject ADGO;
        public int TargetDiffMin = 20;
        public int TargetDiffMax = 36;

        private void Awake()
        {
            timeSelectionHelper = TimeStateManager.Instance.GetComponent<TimeSelectionHelper>();
        }

        private bool TimeInRange()
        {
            return timeSelectionHelper.GetTimeStampDiff >= TargetDiffMin && timeSelectionHelper.GetTimeStampDiff <= TargetDiffMax;
        }

        private void Update()
        {
            if(!timeSelectionHelper.GetTimeFreezeStarted && !TimeInRange())
            {
                timeSelectionHelper.AllowLeaving = false;
                ShiftGO.SetActive(true);
                ShiftLetgoGO.SetActive(false);
                ADGO.SetActive(false);
            }
            if (!timeSelectionHelper.GetTimeFreezeStarted && TimeInRange())
            {
                gameObject.SetActive(false);
                DialogueHelper.Instance.EndDialog();
                DirectorHelper.Instance.ResumeCurrentMovie();
                InputHandler.EnableTimeManipulation(false);
                Time.timeScale = 1f;
            }
            if (timeSelectionHelper.GetTimeFreezeStarted && !TimeInRange())
            {
                timeSelectionHelper.AllowLeaving = false;
                ShiftGO.SetActive(true);
                ShiftLetgoGO.SetActive(false);
                ADGO.SetActive(true);
            }
            if (timeSelectionHelper.GetTimeFreezeStarted && TimeInRange())
            {
                timeSelectionHelper.AllowLeaving = true;
                ShiftGO.SetActive(false);
                ShiftLetgoGO.SetActive(true);
                ADGO.SetActive(false);
            }
        }
    }
}
