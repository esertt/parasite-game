using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public enum DoorColor { Yellow, Blue }
    public DoorColor doorColor;
    public AudioSource lockedSound;
    public DoorTransitionManager transitionManager;
    public string nextChapterScene;
    public float interactionRadius = 1.01f; // distance-based interaction radius
    public string[] interactTags = new string[] { "parasite", "yellowscientist", "bluescientist" };
    public float autoCheckInterval = 0.2f; // seconds between automatic proximity checks
    private float lastAutoCheck = 0f;

    private bool isOpen = false;

    private void Update()
    {
        // Periodically scan for nearby interactors (works without colliders/triggers)
        if (Time.time - lastAutoCheck < autoCheckInterval) return;
        lastAutoCheck = Time.time;

        foreach (string tag in interactTags)
        {
            if (string.IsNullOrEmpty(tag)) continue;
            GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);
            if (candidates == null || candidates.Length == 0) continue;

            foreach (GameObject candidate in candidates)
            {
                if (candidate == null) continue;
                // call TryInteractAt which will check distance and handle matching/opening/locked sound
                TryInteractAt(candidate.transform.position, candidate.tag);
            }
        }
    }

    // Attempts to interact with the door from a given position and interactor tag.
    // Returns true if an interaction was attempted (open or locked), false if out of range or already open.
    public bool TryInteractAt(Vector3 interactorPosition, string interactorTag)
    {
        if (isOpen)
        {
            Debug.Log($"Door.TryInteractAt: door={name} already open; interactorTag={interactorTag}");
            return false;
        }

        float d = Vector3.Distance(transform.position, interactorPosition);
        Debug.Log($"Door.TryInteractAt: door={name} interactorTag={interactorTag} dist={d:F2} radius={interactionRadius}");
        if (d > interactionRadius)
        {
            Debug.Log($"Door.TryInteractAt: door={name} interactor out of range (dist {d:F2})");
            return false; // out of range
        }

        string tagLower = interactorTag != null ? interactorTag.ToLower() : "";
        bool isYellow = tagLower == "yellowscientist";
        bool isBlue = tagLower == "bluescientist";

        if ((doorColor == DoorColor.Yellow && isYellow) || (doorColor == DoorColor.Blue && isBlue))
        {
            Debug.Log($"Door.TryInteractAt: door={name} matched by {interactorTag}; opening and transitioning to {nextChapterScene}");
            OpenDoor();
        }
        else
        {
            Debug.Log($"Door.TryInteractAt: door={name} did NOT match interactorTag={interactorTag}; playing lockedSound");
            if (lockedSound != null) lockedSound.Play();
        }

        return true;
    }

    private void OpenDoor()
    {
        isOpen = true;

        Debug.Log($"{doorColor} door opened!");

        // Trigger scene transition
        if (!string.IsNullOrEmpty(nextChapterScene))
        {
            // Prefer the assigned transitionManager if it's active
            DoorTransitionManager mgr = transitionManager;
            if (mgr == null || (mgr != null && !mgr.gameObject.activeInHierarchy))
            {
                // Try to find any active DoorTransitionManager in the scene
                mgr = UnityEngine.Object.FindFirstObjectByType<DoorTransitionManager>();
                if (mgr != null)
                    Debug.Log($"Door.OpenDoor: using found active DoorTransitionManager on '{mgr.gameObject.name}'");
                else
                    Debug.LogWarning($"Door.OpenDoor: no active DoorTransitionManager found; falling back to direct SceneManager.LoadScene('{nextChapterScene}')");
            }

            if (mgr != null)
            {
                mgr.LoadChapter(nextChapterScene);
            }
            else
            {
                // fallback: load scene directly
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextChapterScene);
            }
        }
    }
}