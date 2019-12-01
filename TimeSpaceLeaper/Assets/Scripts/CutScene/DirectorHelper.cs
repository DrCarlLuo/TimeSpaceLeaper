using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.UI;
using LAB.Mechanics;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using LAB.Engine;

namespace LAB.CutScene
{
    class DirectorHelper : MonoBehaviour
    {
        public static DirectorHelper Instance { get; private set; }
        PlayableDirector director;

        private void Awake()
        {
            Instance = this;
            director = GetComponent<PlayableDirector>();
        }

        public void PauseCurrentMovie()
        {
            director.Pause();
        }

        public void ResumeCurrentMovie()
        {
            director.Resume();
        }

        public void Play(PlayableAsset asset)
        {
            gameObject.SetActive(true);
            director.Play(asset);
        }

        public void Stop()
        {
            director.Stop();
            gameObject.SetActive(false);
        }

        public void GainCameraControll()
        {
            Camera.main.transform.parent.GetComponent<CameraFollow>().enabled = false;
        }

        public void ReleaseCameraControll()
        {
            var comp = Camera.main.transform.parent.GetComponent<CameraFollow>();
            comp.enabled = true;
            comp.CalculateSceneCameraRange();
        }

        public void GainPlayerControll()
        {
            PlayerController.Instance.enabled = false;
            PlayerController.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            PlayerController.Instance.MainTexture.GetComponent<Animator>().SetFloat("HorizontalSpeed", 0f);
            PlayerController.Instance.MainTexture.GetComponent<Animator>().SetBool("IsGrounded", true);
            InputHandler.EnablePlayerControll(false);
            InputHandler.EnableTimeManipulation(false);
        }

        public void ReturnPlayerControl()
        {
            PlayerController.Instance.enabled = true;
            InputHandler.EnablePlayerControll(true);
            InputHandler.EnableTimeManipulation(true);
        }

        public void GainPawnPhysics()
        {
            PlayerController.Instance.GetComponent<Collider2D>().enabled = false;
            PlayerController.Instance.GetComponent<PawnController>().enabled = false;
        }

        public void ReturnPawnPhysics()
        {
            PlayerController.Instance.GetComponent<Collider2D>().enabled = true;
            PlayerController.Instance.GetComponent<PawnController>().enabled = true;
        }
    }
}
