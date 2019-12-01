using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LAB.ManagedTime;
using LAB.Mechanics;

namespace LAB.Engine
{
    class SFXHandle : MonoBehaviour
    {
        public AudioSource audioSource;
        public Coroutine FadeCoroutine;
        public bool PlayableInTimeFreeze = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void StopRightNow()
        {
            audioSource.Stop();
            if(FadeCoroutine!=null)
                StopCoroutine(FadeCoroutine);
            Destroy(gameObject);
        }

        private void Update()
        {
            if (!audioSource.isPlaying)
                Destroy(gameObject);
        }
    }
}
