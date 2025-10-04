using UnityEngine;

public class Lasers : MonoBehaviour
{
    public GameObject LaserProbe1;
    public GameObject LaserProbe2;
    public GameObject LaserRay;
    public bool isOn = true;

    public void TurnOff()
    {
        isOn = false;
        LaserRay.SetActive(false);
    }

    public void TurnOn()
    {
        isOn = true;
        LaserRay.SetActive(true);
    }
}
