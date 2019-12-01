using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LAB.UI;

namespace LAB.Engine
{
    class InputHandler : MonoBehaviour
    {
        #region Input Instances
        //Menu Operation
        public static IButtonInput MenuButton = new TestKeyboardButton(KeyCode.Escape);
        public static IButtonInput MenuUpSwitch = new TestKeyboardButton(KeyCode.UpArrow);
        public static IButtonInput MenuDownSwitch = new TestKeyboardButton(KeyCode.DownArrow);
        public static IButtonInput MenuConfirm = new TestKeyboardButton(KeyCode.Return, KeyCode.Space);
        public static IButtonInput MenuReturn = new TestKeyboardButton(KeyCode.Escape);

        //Player Manipulation
        public static IAxisInput HorizontalAxis = new PreciseKeyboardAxis(KeyCode.LeftArrow, KeyCode.RightArrow);
        public static IButtonInput Jump = new TestKeyboardButton(KeyCode.Space);

        //Time Manipulation
        public static IButtonInput FreezeTime = new TestKeyboardButton(KeyCode.LeftShift);
        public static IButtonInput TimeBackward = new TestKeyboardButton(KeyCode.A);
        public static IButtonInput TimeAfterward = new TestKeyboardButton(KeyCode.D);

        //Other UI Operation
        public static IButtonInput DialogContinue = new TestKeyboardButton(KeyCode.Space);
        #endregion

        public static void EnablePlayerControll(bool enable)
        {
            HorizontalAxis.Enabled = enable;
            Jump.Enabled = enable;
        }

        public static void EnableTimeManipulation(bool enable)
        {
            FreezeTime.Enabled = enable;
            TimeBackward.Enabled = enable;
            TimeAfterward.Enabled = enable;
        }

        private void Update()
        {
            if (MenuButton.KeyDown && GameManager.Instance.GameStateNow == GameState.Running)
                PauseMenuOperation.Instance.OpenMenu();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
