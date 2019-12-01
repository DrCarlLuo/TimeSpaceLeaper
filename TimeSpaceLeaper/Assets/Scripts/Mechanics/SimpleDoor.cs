using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    public class SimpleDoor : ManagedMonoBehaviour
    {
        bool opened;
        public GameObject Switch;
        ISwitchGiver switchGiver;

        protected void Awake()
        {
            switchGiver = Switch.GetComponent<ISwitchGiver>();
        }

        private void Update()
        {
            if (!TimeStateManager.Instance.IsPreview)
                opened = switchGiver.GetSwitchState();
            foreach (Transform x in transform)
            {
                x.gameObject.SetActive(!opened);
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Value(ref opened);
        }
    }
}
