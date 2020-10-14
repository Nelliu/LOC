using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UIButtons
{
    public class BtnFunctions : MonoBehaviour
    {

        public GameObject PauseUI;
        public GameObject ControlsUI;
        public GameObject AboutUI;


        public void Quit()
        {
            Application.Quit();
        }

        public void Continue()
        {
            PauseUI.SetActive(false);
        }

        public void ShowControls()
        {       
            ControlsUI.SetActive(!ControlsUI.activeSelf);

        }

        public void ShowCredits()
        {
            AboutUI.SetActive(!AboutUI.activeSelf);
        }

        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }


    }
}
