using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LAB.Engine;

namespace LAB.UI
{
    class MainMenuOperation : MonoBehaviour
    {
        public GameObject ContinueGO;
        private void Awake()
        {
            ContinueGO.SetActive(SaveManager.Instance.CheckSaveExist());
        }

        public void ContinueGame()
        {
            GameManager.Instance.GoToScene(SaveManager.Instance.GetSceneNameFromSave());
        }

        public void BeginNewGame()
        {
            GameManager.Instance.GoToFirstLevel();
        }

        public void ExitGame()
        {
            GameManager.Instance.ExitGame();
        }
    }
}
