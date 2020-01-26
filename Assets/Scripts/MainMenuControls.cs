using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{
    // Get options
    public GameObject options;

    // Play game
    public void PlayGame()
    {
        // Go to the next scene in the queue (should be the main)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        // Closes the game
        Application.Quit();
    }

    public void OpenOptions()
    {
        options.SetActive(true);
    }
}
