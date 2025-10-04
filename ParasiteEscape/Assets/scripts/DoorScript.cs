using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public enum DoorColor { Yellow, Blue }
    public DoorColor doorColor;
    public AudioSource lockedSound;
    public DoorTransitionManager transitionManager;
    public string nextChapterScene;

    private bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen) return;

        // Check if scientist matches door color
        if ((doorColor == DoorColor.Yellow && other.CompareTag("YellowScientist")) ||
            (doorColor == DoorColor.Blue && other.CompareTag("BlueScientist")))
        {
            OpenDoor();
        }
        else
        {
            // Play locked sound
            if (lockedSound != null)
                lockedSound.Play();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;

        Debug.Log($"{doorColor} door opened!");

        // Trigger scene transition
        if (transitionManager != null && !string.IsNullOrEmpty(nextChapterScene))
            transitionManager.LoadChapter(nextChapterScene);
    }
}