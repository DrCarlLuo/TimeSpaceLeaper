using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    class FireEmitter : ManagedMonoBehaviour
    {
        public float FireDelay = 1f;
        bool fireOn = false;
        Animator animator;
        bool beginCountDown = false;
        float fireRemainingTime = 0f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!beginCountDown && collision.gameObject.tag == "Player" || collision.gameObject.tag == "GhostPlayer")
            {
                if (!fireOn)
                {
                    animator.SetTrigger("Hit");
                    beginCountDown = true;
                    fireRemainingTime = FireDelay;
                }
                else
                {
                    var comp = collision.gameObject.GetComponent<PawnController>();
                    if (comp != null && comp.Alive)
                        comp.Kill(gameObject.transform);
                }
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

        private void Update()
        {
            animator.SetBool("TurnOn", fireOn);
            if (beginCountDown)
            {
                fireRemainingTime -= Time.deltaTime;
                if (fireRemainingTime <= 0f)
                {
                    beginCountDown = false;
                    fireOn = true;
                }
            }
        }

        public override void ExposeTimeData()
        {
            base.ExposeTimeData();
            TimeScribe.Look_Value(ref fireOn);
            TimeScribe.Look_Value(ref beginCountDown);
            TimeScribe.Look_Value(ref fireRemainingTime);
        }
    }
}
