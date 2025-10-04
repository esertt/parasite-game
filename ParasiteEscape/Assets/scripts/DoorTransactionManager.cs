using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorTransitionManager : MonoBehaviour
{
    public CanvasGroup fadeOverlay;       // full-screen black overlay
    public AudioSource chapterSound;      // sound when chapter starts
    public float fadeDuration = 1.5f;

    // Call this from door
    public void LoadChapter(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        // Fade in overlay
        if (fadeOverlay != null)
            yield return StartCoroutine(FadeCanvas(fadeOverlay, 0, 1, fadeDuration));

        // Play chapter sound
        if (chapterSound != null)
            chapterSound.Play();

        // Optional wait to let sound start
        yield return new WaitForSeconds(0.1f);

        // Load next scene
        SceneManager.LoadScene(sceneName);
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