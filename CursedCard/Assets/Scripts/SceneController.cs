using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void NewGameBtn(string newGame)
    {
        SceneManager.LoadScene(newGame);
    }
    public void quittingGame()
    {
        Application.Quit();
    }

}