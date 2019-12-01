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
    class CutSceneEnterTrigger : MonoBehaviour
    {
        public PlayableAsset Movie;
        public bool TriggerOnce;
        bool triggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((!triggered || !TriggerOnce) && collision.gameObject.tag == "Player")
            {
                triggered = true;
                DirectorHelper.Instance.Play(Movie);
            }
        }
    }
}