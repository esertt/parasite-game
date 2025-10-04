using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleAction : MonoBehaviour
{
    [Header("Parasite Setup")]
    public GameObject parasite;
    public Transform movePoint;
    public float speed = 5f;
    private SpriteRenderer sr;
    private string isInfected = "parasite";

    [Header("Sprites")]
    public Sprite scientistSpriteYellow;
    public Sprite scientistSpriteBlue;
    public Sprite babySprite;
    public Sprite soldierSprite;

    [Header("Tilemap & NPCs")]
    public Tilemap tileMap;
    public int targetLayer;
    private List<GameObject> objectsInLayer = new List<GameObject>();

    [Header("NPC Movement")]
    public bool NPCREQUEM = false;

    private bool actionReady = true;

    private void Start()
    {
        isInfected = "parasite";
        movePoint.parent = null;
        sr = parasite.GetComponent<SpriteRenderer>();

        // Gather all objects in the target layer
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == targetLayer)
            {
                objectsInLayer.Add(obj);
                Debug.Log("Found object: " + obj.name);
            }
        }
    }

    private void Update()
    {
        MoveParasite();
        if (actionReady)
        {
            HandleInputMovement();
            CheckForPossession();
            CheckForSoldierThreat();
            if (NPCREQUEM) npcAction();
        }
    }

    private void MoveParasite()
    {
        parasite.transform.position = Vector3.MoveTowards(parasite.transform.position, movePoint.position, speed * Time.deltaTime);
        actionReady = Vector3.Distance(parasite.transform.position, movePoint.position) <= 0f;
    }

    private void HandleInputMovement()
    {
        Vector3 newPos = movePoint.position;
        bool moveNPC = false;

        // Horizontal input
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) == 1)
        {
            newPos += new Vector3(h, 0f, 0f);
            moveNPC = true;
        }

        // Vertical input
        float v = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(v) == 1)
        {
            newPos += new Vector3(0f, v, 0f);
            moveNPC = true;
        }

        // Check if target tile exists
        Vector3Int cellPos = tileMap.WorldToCell(newPos);
        TileBase tile = tileMap.GetTile(cellPos);
        if (tile != null && tile.name == "tiletest_1")
        {
            Debug.Log("Tile found at " + cellPos + ": " + tile.name);
        }

        // Move parasite
        movePoint.position = newPos;

        if (moveNPC) npcAction();
    }

    private void CheckForPossession()
    {
        // Loop backwards for safe removal
        for (int i = objectsInLayer.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsInLayer[i];
            float distance = Vector3.Distance(parasite.transform.position, obj.transform.position);

            if (distance <= 1f) // threshold to possess
            {
                switch (obj.tag)
                {
                    case "scientist":
                        isInfected = "scientist";
                        sr.sprite = scientistSpriteBlue; // adjust for yellow/blue if needed
                        Destroy(obj);
                        objectsInLayer.RemoveAt(i);
                        Debug.Log("Possessed scientist!");
                        break;

                    case "baby":
                        isInfected = "baby";
                        sr.sprite = babySprite;
                        Destroy(obj);
                        objectsInLayer.RemoveAt(i);
                        Debug.Log("Possessed baby!");
                        break;

                    // Soldiers cannot be possessed
                }
            }
        }
    }

    private void CheckForSoldierThreat()
    {
        foreach (GameObject obj in objectsInLayer)
        {
            if (obj.tag == "soldier")
            {
                float distance = Vector3.Distance(parasite.transform.position, obj.transform.position);
                if (distance <= 3f) // threat range
                {
                    Debug.Log("Parasite killed by soldier!");
                    Destroy(parasite);
                    return;
                }
            }
        }
    }

    private void npcAction()
    {
        foreach (GameObject obj in objectsInLayer)
        {
            NPCLogic npc = obj.GetComponent<NPCLogic>();
            if (npc != null)
            {
                npc.Action();
            }
        }
    }
}