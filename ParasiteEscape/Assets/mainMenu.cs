using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace ParasiteEscape
{
public class MainMenu : MonoBehaviour
{
    public Button startButton, optionsButton, quitButton;

    public string optionsSceneName = "Options";
    
    // Audio for UI clicks
    public AudioSource sfxSource; // optional: assign an AudioSource (can be on this GameObject or a singleton audio manager)
    public AudioClip clickSfx; // assign your click sound in Inspector
    [Range(0f,1f)]
    public float clickVolume = 1f;

    // If >0, handlers will wait this many seconds after playing the SFX before loading scenes / quitting.
    // Set to small value like 0.05-0.15 if you want the click to be audible before the scene changes.
    public float sceneLoadDelay = 0.15f;

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
            quitButton.onClick.AddListener(quitGame);
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
            quitButton.onClick.RemoveListener(quitGame);
    }

    // Loads the cinematic scene (set via Inspector). If the field is empty, fall back to next build index.
    public void playGame()
    {
        PlayClickSound();

        // fallback: load next scene in build order
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (sceneLoadDelay > 0f)
            StartCoroutine(LoadSceneAfterDelayIndex(nextIndex));
        else
            SceneManager.LoadScene(nextIndex);
    }

    // Loads options scene (set via Inspector)
    public void options()
    {
        PlayClickSound();

        if (!string.IsNullOrEmpty(optionsSceneName))
        {
            if (sceneLoadDelay > 0f)
                StartCoroutine(LoadSceneAfterDelayName(optionsSceneName));
            else
                SceneManager.LoadScene(optionsSceneName);
            return;
        }

        Debug.LogWarning("mainMenu: optionsSceneName is empty. Please set it in the Inspector.");
    }

    // Quit the application. In the Editor this will log a message.
    public static void QuitGame()
    {
        // kept for backward compatibility in case something still calls static QuitGame
        // play any SFX is not possible from static context; prefer instance quitGame on this component
#if UNITY_EDITOR
        Debug.Log("Application.Quit() called - ignored in Editor.");
#else
        Application.Quit();
#endif
    }

    // Instance quit so we can play click sound before quitting
    public void quitGame()
    {
        PlayClickSound();
        if (sceneLoadDelay > 0f)
            StartCoroutine(QuitAfterDelay(sceneLoadDelay));
        else
        {
#if UNITY_EDITOR
            Debug.Log("Application.Quit() called - ignored in Editor.");
#else
            Application.Quit();
#endif
        }
    }

    private void PlayClickSound()
    {
        if (clickSfx == null) return;
        if (sfxSource != null)
            sfxSource.PlayOneShot(clickSfx, clickVolume);
        else
        {
            Vector3 pos = Vector3.zero;
            if (Camera.main != null) pos = Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(clickSfx, pos, clickVolume);
        }
    }

    private IEnumerator LoadSceneAfterDelayName(string sceneName)
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneAfterDelayIndex(int index)
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(index);
    }

    private IEnumerator QuitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
#if UNITY_EDITOR
        Debug.Log("Application.Quit() called - ignored in Editor.");
#else
        Application.Quit();
#endif
    }

}
}
