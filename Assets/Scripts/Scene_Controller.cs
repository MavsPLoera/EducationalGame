using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Controller : MonoBehaviour
{
    public void loadGame()
    {
        SceneManager.LoadScene("Main_Gameplay_Scene");
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void quit()
    {
        Debug.Log("Quitting Game...");
        //Console.Write()
        Application.Quit();
    }

}
