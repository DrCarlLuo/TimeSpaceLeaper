using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAB.ManagedTime;
using LAB.Mechanics;
using LAB.Engine;
using LAB.VFX;

namespace LAB.UI
{
    public class TimeIndicatorUI : MonoBehaviour
    {
        public Text MainText;
        public GameObject AgoGO;
        public GameObject NowGO;
        public Image Ring;

        public Color EarlyColor = Color.cyan;
        public Color LateColor = Color.red;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void SetTimeValue(int value, int minValue, int maxValue, string timeStr)
        {
            MainText.text = timeStr;
            MainText.gameObject.SetActive(value != 0);
            AgoGO.SetActive(value != 0);
            NowGO.SetActive(value == 0);

            float length = maxValue - minValue;
            if (length == 0) length = 1f;
            float ratio = Mathf.Clamp01((value - minValue) / length);
            Ring.fillAmount = 1 - ratio;

            MainText.color = Color.Lerp(EarlyColor, LateColor, ratio);
        }
    }
}
