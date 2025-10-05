using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DoorTransitionManager : MonoBehaviour
{
    public AudioSource chapterSound;      // sound when chapter starts
    public float preLoadDelay = 0.1f;     // optional short delay to let sound start

    // make sure manager persists so audio isn't destroyed during scene load
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Call this from door
    public void LoadChapter(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        // Play chapter sound if assigned
        if (chapterSound != null)
            chapterSound.Play();

        // optional small delay so the sound has time to start
        if (preLoadDelay > 0f)
            yield return new WaitForSeconds(preLoadDelay);

        // Load next scene immediately
        LevelTracker.Instance.currentLevel = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    // Fade removed — transitions are now just sound + immediate scene load
}