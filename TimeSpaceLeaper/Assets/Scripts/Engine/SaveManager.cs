using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LAB.Engine
{
    class SaveManager
    {
        private static SaveManager instance = null;
        public static SaveManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SaveManager();
                return instance;
            }
        }
        private SaveManager() { }

        string savePath = UnityEngine.Application.dataPath + "/save.txt";

        public bool CheckSaveExist()
        {
#if UNITY_EDITOR
            return false;
#else
            return File.Exists(savePath);
#endif
        }

        public string GetSceneNameFromSave()
        {
#if UNITY_EDITOR
            return "MainMenu";
#else
            string text = File.ReadAllText(savePath).Trim();
            return text;
#endif
        }

        public void ScribeSave(string sceneName)
        {
#if UNITY_EDITOR
            return;
#else
            //UnityEngine.Debug.Log(UnityEngine.Application.dataPath);
            File.WriteAllLines(savePath, new string[] { sceneName });
#endif
        }
    }
}
