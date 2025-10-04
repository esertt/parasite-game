using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class puzzleAction : MonoBehaviour
{
    private string isInfected;
    private bool actionReady;
    private List<GameObject> objectsInLayer = new List<GameObject>();
    private SpriteRenderer sr;
    private Sprite sprite;

    public int targetLayer;
    public float speed = 5.0f;
    public Transform movePoint;
    public Tilemap tileMap;
    public GameObject parasite;
    public bool NPCREQUEM = false;
    public Sprite scientistSpriteYellow;
    public Sprite scientistSpriteBlue;
    public Sprite babySprite;
    public Sprite soldierSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInfected = "parasite";
        movePoint.parent = null;
        actionReady = true;
        sr = parasite.GetComponent<SpriteRenderer>();

        // Get all objects in the scene
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == targetLayer)
            {
                objectsInLayer.Add(obj);
                Debug.Log(obj.name);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        parasite.transform.position = Vector3.MoveTowards(parasite.transform.position, movePoint.position, speed * Time.deltaTime);


        if (Vector3.Distance(parasite.transform.position, movePoint.position) <= 0f) actionReady = true;
        else actionReady = false;
        if (actionReady)
        {
            bool moveNPC = false;
            //first update the desire path
            Vector3 newPos = movePoint.position;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
            {
                newPos = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                moveNPC = true;
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
            {
                newPos = movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                moveNPC = true;
            }

            //check the desired path. If player can move then update the movePosition
            Vector3Int cellPos = tileMap.WorldToCell(newPos);
            TileBase tile = tileMap.GetTile(cellPos);
            if (tile != null && tile.name == "tiletest_1")
            {
                Debug.Log("Tile found at " + cellPos + ": " + tile.name);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < objectsInLayer.Count; i++)
                {
                    GameObject obj = objectsInLayer[i];

                    if (Vector3.Distance(parasite.transform.position, obj.transform.position) <= 2.07)
                    {
                        Debug.Log("Destroying and removing: " + obj.name);

                        // --- Action: Destroy the GameObject in the scene ---
                        Destroy(obj);

                        // --- Action: Remove the reference from the C# list ---
                        objectsInLayer.RemoveAt(i);

                        // Your existing logic for movement/targeting
                        movePoint.position = obj.transform.position;
                        switch (obj.tag)
                        {
                            case "scientist":
                                isInfected = "scientist"; sr.sprite = scientistSpriteBlue; break;
                            case "soldier":
                                isInfected = "sodlier"; sr.sprite = soldierSprite; break;
                            case "baby":
                                isInfected = " baby"; sr.sprite = babySprite; break;
                        }

                        // 💡 OPTIONAL: Since you found one and destroyed it, 
                        // you can break out of the loop if you don't want to check others.
                        break;
                    }
                }
                npcAction();
            }
            else
            {
                movePoint.position = newPos;
                if (moveNPC || NPCREQUEM) npcAction();
            }
        }
    }

    private void npcAction()
    {
        foreach (GameObject obj in objectsInLayer)
        {
            NPCLogic npc = obj.GetComponent<NPCLogic>();
            if (npc != null) // Component varsa çalıştır
            {
                npc.Action();
            }
        }
    }
}
