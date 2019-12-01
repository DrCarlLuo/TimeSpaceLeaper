using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Mechanics;
using UnityEngine.SceneManagement;

namespace LAB.Engine
{
    public class SceneRoot : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            if (GameManager.TestLoadSceneName == "")
            {
                GameManager.TestLoadSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("Main");
                Destroy(this);
            }
        }
#endif

        public Transform PlayerSpawnPoint;

        private void Start()
        {
            SaveManager.Instance.ScribeSave(SceneManager.GetActiveScene().name);
        }

        public void RegisterAllSceneElementForTime()
        {
            var comps = GetComponentsInChildren<ITimeManaged>();
            foreach (var c in comps)
                TimeStateManager.Instance.Register(c);
        }
    }
}
