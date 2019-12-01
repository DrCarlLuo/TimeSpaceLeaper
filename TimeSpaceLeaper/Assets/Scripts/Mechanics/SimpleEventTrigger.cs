using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    class SimpleEventTrigger : MonoBehaviour
    {
        public UnityEvent Events;
        public bool TriggerOnce;
        bool triggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((!triggered || !TriggerOnce) && collision.gameObject.tag == "Player")
            {
                triggered = true;
                Events.Invoke();
            }
        }
    }
}
