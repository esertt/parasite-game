using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class SceneTransitionController : MonoBehaviour
{
    public CanvasGroup mainMenu;
    public CanvasGroup fadeOverlay;
    public RawImage smallVideoImage;   // RawImage to display video
    public VideoPlayer smallVideoPlayer; // VideoPlayer component
    public AudioSource transitionSound;
    public float fadeDuration = 1.5f;
    public string nextSceneName;

    public void OnStartButtonClicked()
    {
        StartCoroutine(TransitionToChapter());
    }

    private IEnumerator TransitionToChapter()
    {
        // Fade out main menu
        yield return StartCoroutine(FadeCanvas(mainMenu, 1, 0, fadeDuration / 2));

        // Play sound if assigned
        if (transitionSound != null)
            transitionSound.Play();

        // Fade to black
        yield return StartCoroutine(FadeCanvas(fadeOverlay, 0, 1, fadeDuration));

        // Play video
        if (smallVideoPlayer != null && smallVideoImage != null)
        {
            smallVideoImage.gameObject.SetActive(true);
            smallVideoPlayer.Play();
            while (smallVideoPlayer.isPlaying)
            {
                yield return null;
            }
            smallVideoImage.gameObject.SetActive(false);
        }

        // Load first chapter
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