using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.Mechanics
{
    public class SimpleSpike : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var comp = collision.gameObject.GetComponent<PawnController>();
            if (comp != null && comp.Alive)
                comp.Kill(gameObject.transform);
        }
    }
}
