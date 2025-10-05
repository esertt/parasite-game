using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Tooltip("Scene name for the first chapter. Make sure this scene is added to Build Settings.")]
    public string firstChapterSceneName = "Chapter1";

    // Hook this to the Button OnClick in the GameOver scene
    public void LoadFirstChapter()
    {
        if (string.IsNullOrEmpty(firstChapterSceneName))
        {
            Debug.LogWarning("GameOverMenu.LoadFirstChapter: firstChapterSceneName is empty. Set it in the Inspector or call LoadSceneByName.");
            return;
        }
        SceneManager.LoadScene(firstChapterSceneName);
    }

    // Generic helper if you want to pass scene name directly from the Inspector
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("GameOverMenu.LoadSceneByName: sceneName was empty");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
