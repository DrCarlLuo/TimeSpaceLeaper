using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LAB.ManagedTime;
using LAB.Mechanics;
using LAB.UI;

namespace LAB.Engine
{
    public enum GameState
    {
        Undefined,
        Running,
        Paused,
        MainMenu
    }

    public class GameManager : MonoBehaviour
    {
        #region Engine Prefabs
        public GameObject TimeStateManagerPrefab;
        public GameObject GhostManagerPrefab;
        public GameObject PlayerPrefab;
        public GameObject FXManagerPrefab;
        public GameObject PauseMenuPrefab;
        public GameObject SoundManagerPrefab;
        #endregion

        #region Parameters
        public string FirstSceneName;
        #endregion

        #region Easy Access
        public GameObject PlayerGO
        {
            get
            {
                if (PlayerController.Instance != null)
                    return PlayerController.Instance.gameObject;
                return null;
            }
        }
        public TimeSelectionHelper TimeSelectionHelperInt { get; private set; }
        #endregion

        #region Singleton
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
            {
                Log.Warning("Try to create multiple GameManager!");
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        public GameState GameStateNow;
        public SceneRoot CurrentSceneRoot = null;
        public float TimeScaleBeforePause;
        

        #region Scene Loading
        public void GoToFirstLevel()
        {
            StartCoroutine(AsyncLoadScene(FirstSceneName));
        }

        IEnumerator AsyncLoadScene(string sceneName)
        {
            Time.timeScale = 1f;//Sometimes scene will be reloaded from a freeze state
            if (TimeStateManager.Instance != null)
                TimeStateManager.Instance.UnregisterAll();
            yield return SceneManager.LoadSceneAsync(sceneName);

            if (SoundManager.Instance == null)
                Instantiate(SoundManagerPrefab, Vector3.zero, Quaternion.identity);
            else
                SoundManager.Instance.ClearAllSFX();

            Instantiate(TimeStateManagerPrefab, Vector3.zero, Quaternion.identity);
            TimeSelectionHelperInt = TimeStateManager.Instance.GetComponent<TimeSelectionHelper>();
            Instantiate(GhostManagerPrefab, Vector3.zero, Quaternion.identity);

            GameObject go = GameObject.Find("/SceneRoot");
            if (go == null)
            {
                Log.Error("No Scene Root in Scene!");
                yield break;
            }
            CurrentSceneRoot = go.GetComponent<SceneRoot>();
            CurrentSceneRoot.RegisterAllSceneElementForTime();

            if (CurrentSceneRoot.PlayerSpawnPoint != null)
            {
                Instantiate(PlayerPrefab, CurrentSceneRoot.PlayerSpawnPoint.position, Quaternion.identity);
                ManagedTimeUtilities.RegisterEntireGO(PlayerController.Instance.gameObject);
            }

            Instantiate(FXManagerPrefab, Vector3.zero, Quaternion.identity);
            TimeStateManager.Instance.BeginRecord();
            GameStateNow = GameState.Running;
            yield return null;
            Instantiate(PauseMenuPrefab, Vector3.zero, Quaternion.identity);
        }

        public void LoadMainMenu()
        {
            GameStateNow = GameState.MainMenu;
            if (SoundManager.Instance != null)
                SoundManager.Instance.ClearAllSFX();
            SceneManager.LoadScene("MainMenu");
        }

        public void ReloadCurrentScene()
        {
            StartCoroutine(AsyncLoadScene(SceneManager.GetActiveScene().name));
        }

        public void GoToScene(string sceneName)
        {
            StartCoroutine(AsyncLoadScene(sceneName));
        }
        #endregion

        #region Game State Manipulation
        public void PauseGame()
        {
            TimeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            GameStateNow = GameState.Paused;
        }

        public void UnPauseGame()
        {
            Time.timeScale = TimeScaleBeforePause;
            GameStateNow = GameState.Running;
        }

        public void ExitGame()
        {
            Application.Quit();
        }
        #endregion

#if UNITY_EDITOR
        public static string TestLoadSceneName = "";
#endif

        private void Start()
        {
#if UNITY_EDITOR
            if (TestLoadSceneName != "" && TestLoadSceneName != "!")
                GoToScene(TestLoadSceneName);
            else
            {
#endif
                LoadMainMenu();
#if UNITY_EDITOR
            }
                TestLoadSceneName = "!";
#endif
        }
    }
}
