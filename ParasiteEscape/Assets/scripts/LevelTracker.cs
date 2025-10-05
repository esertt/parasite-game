using UnityEngine;

public class LevelTracker : MonoBehaviour
{
    public static LevelTracker Instance;
    public string currentLevel = "Chapter1";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
