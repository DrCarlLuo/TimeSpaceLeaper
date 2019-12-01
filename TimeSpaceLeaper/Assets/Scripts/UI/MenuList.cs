using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LAB.Engine;

namespace LAB.UI
{
    class MenuList : MonoBehaviour
    {
        int currentSelection;
        List<MenuItemText> choices;
        public MenuItemText defaultOne = null;

        private void Awake()
        {
            currentSelection = 0;
        }

        private void Start()
        {
            choices = new List<MenuItemText>();
            foreach (Transform item in transform)
            {
                if (item.gameObject.activeSelf)
                    choices.Add(item.GetComponent<MenuItemText>());
            }
            if (defaultOne == null)
                choices[currentSelection].Select();
            else
                defaultOne.Select();
        }

        private void Update()
        {
            if(InputHandler.MenuUpSwitch.KeyDown)
            {
                choices[currentSelection].DeSelect();
                --currentSelection;
                if (currentSelection < 0)
                    currentSelection = choices.Count - 1;
                choices[currentSelection].Select();
            }
            if(InputHandler.MenuDownSwitch.KeyDown)
            {
                choices[currentSelection].DeSelect();
                ++currentSelection;
                if (currentSelection >= choices.Count)
                    currentSelection = 0;
                choices[currentSelection].Select();
            }
            if (InputHandler.MenuConfirm.KeyDown)
                choices[currentSelection].Confirm();
        }
    }
}
