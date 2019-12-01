using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LAB.Engine;

namespace LAB.UI
{
    class PauseMenuOperation : MonoBehaviour
    {
        public static PauseMenuOperation Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void OpenMenu()
        {
            gameObject.SetActive(true);
            GameManager.Instance.PauseGame();
        }

        public void CloseMenu()
        {
            gameObject.SetActive(false);
            GameManager.Instance.UnPauseGame();
        }

        public void RestartLevel()
        {
            CloseMenu();
            GameManager.Instance.ReloadCurrentScene();
        }

        public void GoToMainMenu()
        {
            CloseMenu();
            GameManager.Instance.LoadMainMenu();
        }

        public void GoToDeskTop()
        {
            GameManager.Instance.ExitGame();
        }

        private void Update()
        {
            if (InputHandler.MenuReturn.KeyDown)
                CloseMenu();
        }
    }
}
