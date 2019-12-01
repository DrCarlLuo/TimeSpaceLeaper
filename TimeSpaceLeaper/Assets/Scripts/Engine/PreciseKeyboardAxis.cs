using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.Engine
{
    class PreciseKeyboardAxis : IAxisInput
    {
        private KeyCode negativeKey;
        private KeyCode positiveKey;

        public bool Enabled { get; set; }

        public float Value => GetAxis();

        private float GetAxis()
        {
            if (!Enabled) return 0f;
            if (Input.GetKey(negativeKey) && Input.GetKey(positiveKey))
                return 0f;
            if (Input.GetKey(negativeKey))
                return -1f;
            if (Input.GetKey(positiveKey))
                return 1f;
            return 0f;
        }

        public PreciseKeyboardAxis(KeyCode negative, KeyCode positive)
        {
            Enabled = true;
            negativeKey = negative;
            positiveKey = positive;
        }
    }
}
