using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public GameObject quitDialog;
    public GameObject settingsDialog;

    public GameObject sandboxButton;
    public GameObject walkthroughsButton;
    public GameObject backButton;
    public GameObject quitButton;

    public GameObject walkthroughsScrollView;

    public GameObject buttonPrefab; // Assign the WalkthroughButton prefab in the Inspector
    public Transform contentArea;  // Assign the "Content" object of the Scroll View here
    public string folderPath; // Folder name in Resources or StreamingAssets

    private bool isPopulated = false; // Track if the menu is already populated


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

    public void ShowWalkthroughs()
    {
        sandboxButton.SetActive(false);
        walkthroughsButton.SetActive(false);
        quitButton.SetActive(false);

        backButton.SetActive(true);
        walkthroughsScrollView.SetActive(true);

        if (!isPopulated) // Only populate once
        {
            LoadWalkthroughButtons();
            isPopulated = true;
        }
    }

    private void LoadWalkthroughButtons()
    {
        // Resolve the folder path
        string fullPath = Path.Combine(Application.dataPath, folderPath);

        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"Directory not found: {fullPath}");
            return;
        }

        // Get all files in the folder
        string[] files = Directory.GetFiles(fullPath);

        foreach (string file in files)
        {
            if (Path.GetExtension(file) == ".meta")
                continue;

            string fileName = Path.GetFileNameWithoutExtension(file); // Get the file name

            // Create a button for each file
            GameObject button = Instantiate(buttonPrefab, contentArea);
            button.GetComponentInChildren<TMP_Text>().text = fileName; // Set button text

            // Add click listener
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(file));
        }
    }

    private void OnButtonClick(string filePath)
    {
        Debug.Log($"Clicked: {filePath}");
        // Add logic to load or display the walkthrough file
    }

    public void HideWalkthroughs()
    {
        walkthroughsScrollView.SetActive(false);
        backButton.SetActive(false);

        quitButton.SetActive(true);
        sandboxButton.SetActive(true);
        walkthroughsButton.SetActive(true);
    }

}
