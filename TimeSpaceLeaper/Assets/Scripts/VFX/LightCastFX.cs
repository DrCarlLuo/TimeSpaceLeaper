using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;

namespace LAB.VFX
{
    class LightCastFX : MonoBehaviour
    {
        public void PostSpawn()
        {
            transform.localScale = new Vector3(6f, 6f, 1f);
            this.ManagedDestroy(gameObject, 1.5f);
        }
    }
}
