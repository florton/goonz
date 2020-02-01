using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startmen : MonoBehaviour
{
     

    public void startgame()
    {
        SceneManager.LoadScene(1);
    }

    public void quitgame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }


 
}