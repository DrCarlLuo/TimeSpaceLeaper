using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LAB.ManagedTime;

namespace LAB.QA
{
    class TimeTestWindow : MonoBehaviour
    {
        private Rect windowRect = new Rect(120, 20, 120, 100);

        public UnityEvent MagicEvent = new UnityEvent();

        void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, WindowFunction, "Tests");
        }

        void WindowFunction(int windowID)
        {
            if(GUI.Button(new Rect(10,10,20,10),"Magic"))
            {
                MagicEvent.Invoke();
            }
        }

    }


}
