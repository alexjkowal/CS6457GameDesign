using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void OnStartGame()
    {
        SceneManager.LoadScene("Scenes/LevelOne");
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnBackToHome()
    {
        SceneManager.LoadScene("Scenes/StartScreen");
    }

    public void OnTutorial()
    {
        SceneManager.LoadScene("Scenes/TutorialScene");
    }
}
