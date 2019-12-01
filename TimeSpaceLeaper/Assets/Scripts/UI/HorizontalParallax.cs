using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.Engine;

namespace LAB.UI
{
    class HorizontalParallax : MonoBehaviour
    {
        GameObject camRoot;
        float length, startPos;
        public float parallaxEffect;

        private void Start()
        {
            camRoot = Camera.main.transform.parent.gameObject;
            startPos = transform.position.x;
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void Update()
        {
            float temp = (camRoot.transform.position.x * (1 - parallaxEffect));
            float dist = (camRoot.transform.position.x * parallaxEffect);
            transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

            if (temp > startPos + length) startPos += length;
            else if (temp < startPos - length) startPos -= length;
        }

    }
}
