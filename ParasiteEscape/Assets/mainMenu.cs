using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ParasiteEscape
{
public class MainMenu : MonoBehaviour
{
    public Button startButton, optionsButton, quitButton;

    // Configurable scene names so you can set them in the Inspector
    public string cinematicSceneName = "Cinematic"; // scene played before chapter 1
    public string optionsSceneName = "Options";

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(playGame);
        else
            Debug.LogWarning("mainMenu: startButton is not assigned in the Inspector.");

        if (optionsButton != null)
            optionsButton.onClick.AddListener(options);
        else
            Debug.LogWarning("mainMenu: optionsButton is not assigned in the Inspector.");

        if (quitButton != null)
            quitButton.onClick.AddListener(MainMenu.QuitGame);
        else
            Debug.LogWarning("mainMenu: quitButton is not assigned in the Inspector.");
    }

    void OnDestroy()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(playGame);
        if (optionsButton != null)
            optionsButton.onClick.RemoveListener(options);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(MainMenu.QuitGame);
    }

    // Loads the cinematic scene (set via Inspector). If the field is empty, fall back to next build index.
    public void playGame()
    {
        if (!string.IsNullOrEmpty(cinematicSceneName))
        {
            SceneManager.LoadScene(cinematicSceneName);
            return;
        }

        // fallback: load next scene in build order
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Loads options scene (set via Inspector)
    public void options()
    {
        if (!string.IsNullOrEmpty(optionsSceneName))
        {
            SceneManager.LoadScene(optionsSceneName);
            return;
        }

        Debug.LogWarning("mainMenu: optionsSceneName is empty. Please set it in the Inspector.");
    }

    // Quit the application. In the Editor this will log a message.
    public static void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Application.Quit() called - ignored in Editor.");
#else
        Application.Quit();
#endif
    }

}
}
