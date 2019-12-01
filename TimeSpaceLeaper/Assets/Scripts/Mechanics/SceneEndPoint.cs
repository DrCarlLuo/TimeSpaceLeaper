using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.ManagedTime;
using LAB.Engine;

namespace LAB.Mechanics
{
    class SceneEndPoint : MonoBehaviour
    {
        public string SceneName;
        public bool BackToMenu = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.tag == "Player")
            {
                if (BackToMenu)
                    GameManager.Instance.LoadMainMenu();
                else
                    GameManager.Instance.GoToScene(SceneName);
            }
        }
    }
}
