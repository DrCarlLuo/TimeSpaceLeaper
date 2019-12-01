
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LAB.AI;
using LAB.ManagedTime;
using LAB.Mechanics;

namespace LAB.UI
{
    class DialogueHelper : MonoBehaviour
    {
        public static DialogueHelper Instance { get; private set; }
        public GameObject ContinueGO;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public Text MainText = null;

        public void ShowText(string x, bool showContinueGO = true)
        {
            MainText.text = x;
            gameObject.SetActive(true);
            ContinueGO.SetActive(showContinueGO);
        }

        public void EndDialog()
        {
            gameObject.SetActive(false);
        }
    }
}
