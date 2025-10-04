using UnityEngine;
using UnityEngine.UI;

public class changeImage : MonoBehaviour
{
    public Sprite state1;      // First sprite
    public Sprite state2;      // Second sprite

    private Image image;
    private bool isState1 = true;  // Tracks current state

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("No Image component found on " + gameObject.name);
        }

        // Initialize with state1
        if (state1 != null)
            image.sprite = state1;
    }

    /// <summary>
    /// Toggles the image between the two states.
    /// </summary>
    public void Toggle()
    {
        if (image == null) return;

        if (isState1)
        {
            if (state2 != null) image.sprite = state2;
            isState1 = false;
        }
        else
        {
            if (state1 != null) image.sprite = state1;
            isState1 = true;
        }
    }
}