using UnityEngine;

public class LaserController : MonoBehaviour
{
    public enum LaserColor { Yellow, Blue };
    public LaserColor laserColor = LaserColor.Yellow;
    public bool isOn = true;
    public Sprite onSprite;
    public Sprite offSprite;

    public Lasers lasers;

    SpriteRenderer spriteRenderer;

    public float interactionRadius = 1.01f; // distance-based interaction radius
    public string[] interactTags = new string[] { "parasite", "yellowscientist", "bluescientist" };
    public float autoCheckInterval = 0.2f; // seconds between automatic proximity checks
    private float lastAutoCheck = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isOn)
        {
            spriteRenderer.sprite = onSprite;
        }
        else
        {
            spriteRenderer.sprite = offSprite;
        }
    }

    void TurnOff()
    {
        isOn = false;
        spriteRenderer.sprite = offSprite;
        lasers.TurnOff();
    }

    // Update is called once per frame
    void Update()
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

    public bool TryInteractAt(Vector3 interactorPosition, string interactorTag)
    {
        if (!isOn)
        {
            // Debug.Log($"LaserController.TryInteractAt: laser={name} already off; interactorTag={interactorTag}");
            return false;
        }

        float d = Vector3.Distance(transform.position, interactorPosition);
        // Debug.Log($"LaserController.TryInteractAt: laser={name} interactorTag={interactorTag} dist={d:F2} radius={interactionRadius}");
        if (d > interactionRadius)
        {
            // Debug.Log($"LaserController.TryInteractAt: laser={name} interactor out of range (dist {d:F2})");
            return false; // out of range
        }

        string tagLower = interactorTag != null ? interactorTag.ToLower() : "";
        bool isYellow = tagLower == "yellowscientist";
        bool isBlue = tagLower == "bluescientist";

        if ((laserColor == LaserColor.Yellow && isYellow) || (laserColor == LaserColor.Blue && isBlue))
        {
            // Debug.Log($"LaserController.TryInteractAt: laser={name} matched by {interactorTag}; turning off");
            TurnOff();
        }
        else
        {
            // Debug.Log($"LaserController.TryInteractAt: laser={name} did NOT match interactorTag={interactorTag}; playing lockedSound");
            // if (lockedSound != null) lockedSound.Play();
        }

        return true;
    }
}
