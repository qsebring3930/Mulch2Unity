using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject quitDialog;
    public GameObject settingsDialog;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Show the confirmation dialog
        quitDialog.SetActive(true);
    }

    public void Settings()
    {
        settingsDialog.SetActive(true);
    }

    public void ExitSettings()
    {
        settingsDialog.SetActive(false);
    }

    public void ConfirmQuit()
    {
        // Quit the application
        Application.Quit();
        Debug.Log("Game is exiting...");
    }

    public void CancelQuit()
    {
        // Hide the confirmation dialog
        quitDialog.SetActive(false);
    }
}
