using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAB.AI;
using LAB.ManagedTime;
using LAB.Mechanics;

namespace LAB.UI
{
    class GhostCountDown : MonoBehaviour
    {
        Text text;
        public PlayerGhostAI playerGhostAI;
        bool started = false;

        public void Awake()
        {
            text = GetComponent<Text>();
        }

        public void StartCoundDown()
        {
            started = true;
        }

        private void Update()
        {
            if (PlayerGhostManager.Instance.EnabledGhostCnt > TimeStateManager.Instance.GetComponent<TimeSelectionHelper>().MaxGhostNum)
                text.text = "TOO MANY!!!";
            else if (started)
            {
                float currentTime = 0f;
                if (TimeStateManager.Instance.IsPreview)
                    currentTime = TimeStateManager.Instance.PreviewTime;
                else
                    currentTime = TimeStateManager.Instance.FakeTime;
                float dif = playerGhostAI.LatestDestroyTime - currentTime;
                dif = Mathf.Min(playerGhostAI.ExistTime, dif);
                text.text = dif.ToString("F1");
            }
        }
    }
}
