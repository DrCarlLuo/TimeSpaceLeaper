using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    class IntervalFireEmitter : ManagedMonoBehaviour
    {
        public float CoolDownTime = 2f;
        public float FireExistTime = 2f;
        public bool StartWithFire = false;
        bool fireOn = false;
        Animator animator;
        float remainingTime = 0f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if(StartWithFire)
            {
                fireOn = true;
                remainingTime = FireExistTime;
            }
            else
            {
                fireOn = false;
                remainingTime = CoolDownTime;
            }
        }

        private void Update()
        {
            animator.SetBool("TurnOn", fireOn);
            remainingTime -= Time.deltaTime;
            if(remainingTime <= 0f)
            {
                if (fireOn)
                    remainingTime = CoolDownTime;
                else
                    remainingTime = FireExistTime;
                fireOn = !fireOn;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (fireOn && collision.gameObject.tag == "Player" || collision.gameObject.tag == "GhostPlayer")
            {
                var comp = collision.gameObject.GetComponent<PawnController>();
                if (comp != null && comp.Alive)
                    comp.Kill(gameObject.transform);
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Value(ref fireOn);
            TimeScribe.Look_Value(ref remainingTime);
        }
    }
}
