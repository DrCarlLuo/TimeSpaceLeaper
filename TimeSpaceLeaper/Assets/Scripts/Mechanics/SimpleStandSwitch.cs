using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Engine;

namespace LAB.Mechanics
{
    class SimpleStandSwitch : MonoBehaviour,ISwitchGiver
    {
        Animator animator;
        public int InAreaNum = 0;

        void Awake(){
            animator = transform.GetComponent<Animator>();

        }
        private void Start()
        {
            InAreaNum = 0;
        }

        public bool GetSwitchState()
        {
            return InAreaNum > 0;
        }

        private void Update()
        {
            animator.SetBool("active", InAreaNum > 0);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "GhostPlayer")
            {
                if (InAreaNum == 0)
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.SwitchSound, 1f);
                ++InAreaNum;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "GhostPlayer")
                --InAreaNum;
        }
    }
}
