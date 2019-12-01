using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LAB
{
  public class Menu : MonoBehaviour
  {
      public Button okButton;
      public Button cancelButton;
      void Start(){
        okButton.onClick.AddListener(Ok);
        cancelButton.onClick.AddListener(Cancel);
      }
      void Ok(){
        Application.Quit();
      }
      void Cancel(){
        transform.gameObject.SetActive(false);
      }
  }
}
