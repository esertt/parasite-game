using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionController : MonoBehaviour
{
    public CanvasGroup mainMenu;
    public CanvasGroup fadeOverlay;
    public AudioSource transitionSound;
    public float fadeDuration = 1.5f;
    public string nextSceneName = "LoreScene1";

    public void OnStartButtonClicked()
    {
        StartCoroutine(TransitionToChapter());
    }

    private IEnumerator TransitionToChapter()
    {
        // Start the fade-out of main menu
        yield return StartCoroutine(FadeCanvas(mainMenu, 1, 0, fadeDuration / 2));

        // Play sound (optional)
        if (transitionSound != null)
            transitionSound.Play();

        // Fade to black
        yield return StartCoroutine(FadeCanvas(fadeOverlay, 0, 1, fadeDuration));

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeCanvas(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = end;
    }
}