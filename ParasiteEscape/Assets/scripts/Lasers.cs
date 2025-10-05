using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Lasers : MonoBehaviour
{
    public GameObject LaserProbe1;
    public GameObject LaserProbe2;
    [DoNotSerialize] public GameObject[] LaserRays;
    public bool isOn = true;

    void Start()
    {
        var rays = new List<GameObject>();
        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (t.gameObject.CompareTag("Ray"))
                rays.Add(t.gameObject);
        }
        LaserRays = rays.ToArray();
    }

    public void TurnOff()
    {
        isOn = false;
        foreach (GameObject laserRay in LaserRays)
        {
            laserRay.SetActive(false);
        }
    }

    public void TurnOn()
    {
        isOn = true;
        foreach (GameObject laserRay in LaserRays)
        {
            laserRay.SetActive(true);
        }
    }
}
