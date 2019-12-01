using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LAB.Engine
{
    class TestKeyboardButton : IButtonInput
    {
        private KeyCode mainKey;
        private KeyCode altKey;
        

        public bool Enabled { get; set; }
        public bool KeyHold => Enabled && (Input.GetKey(mainKey) || Input.GetKey(altKey));
        public bool KeyDown => Enabled && (Input.GetKeyDown(mainKey) || Input.GetKeyDown(altKey));
        public bool KeyUp => Enabled && (Input.GetKeyUp(mainKey) || Input.GetKeyUp(altKey));

        public TestKeyboardButton(KeyCode newKey, KeyCode secondKey = KeyCode.None)
        {
            Enabled = true;
            mainKey = newKey;
            altKey = secondKey;
        }
    }
}
