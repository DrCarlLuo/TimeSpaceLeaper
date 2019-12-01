using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Mechanics;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace LAB.CutScene
{
    class CutSceneSwitchTrigger : MonoBehaviour
    {
        public PlayableAsset Movie;
        public bool TriggerOnce;
        bool triggered = false;

        ISwitchGiver switchGiver;

        private void Awake()
        {
            switchGiver = GetComponent<ISwitchGiver>();
        }

        private void Update()
        {
            if(switchGiver.GetSwitchState() && (!triggered || !TriggerOnce))
            {
                triggered = true;
                DirectorHelper.Instance.Play(Movie);
            }
        }
    }
}
