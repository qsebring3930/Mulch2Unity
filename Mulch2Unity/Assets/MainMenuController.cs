using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject quitDialog; // Assign the QuitConfirmationDialog in the Inspector.

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Show the confirmation dialog
        quitDialog.SetActive(true);
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
