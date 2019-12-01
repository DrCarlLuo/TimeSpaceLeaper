using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LAB.Engine;

namespace LAB.UI
{
    class MenuItemText : MonoBehaviour
    {
        string orgText;
        Text text;
        public UnityEvent Events;

        private void Awake()
        {
            text = GetComponent<Text>();
            orgText = text.text;
        }

        public void Select()
        {
            text.text = "> " + orgText;
        }

        public void DeSelect()
        {
            text.text = orgText;
        }

        public void Confirm()
        {
            Events.Invoke();
        }
    }
}
