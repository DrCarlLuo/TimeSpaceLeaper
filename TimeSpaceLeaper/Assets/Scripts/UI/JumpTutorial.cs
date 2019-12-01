using System;
using System.Collections.Generic;
using UnityEngine;
using LAB.Engine;

namespace LAB.UI
{
    class JumpTutorial : MonoBehaviour
    {
        private void Update()
        {
            if (InputHandler.Jump.KeyDown)
                gameObject.SetActive(false);
        }
    }
}
