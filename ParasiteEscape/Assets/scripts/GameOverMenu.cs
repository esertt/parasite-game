using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{

    // Hook this to the Button OnClick in the GameOver scene
    public void RestartLevel()
    {
        if (LevelTracker.Instance != null)
        {
            SceneManager.LoadScene(LevelTracker.Instance.currentLevel);
        }
        else
        {
            SceneManager.LoadScene("Chapter1");
        }
    }
}
