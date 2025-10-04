using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class puzzleAction : MonoBehaviour
{
    private bool isInfected;
    private GameObject objToPossess;
    private bool canPossess;
    private bool actionReady;
    private List<GameObject> objectsInLayer = new List<GameObject>();

    public int targetLayer;
    public float speed = 5.0f;
    public Transform movePoint;
    public Tilemap tileMap;
    public GameObject parasite;
    public bool NPCREQUEM = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInfected = false;
        movePoint.parent = null;
        actionReady = true;

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
        foreach (GameObject obj in objectsInLayer)
        {
            if (Vector3.Distance(parasite.transform.position, obj.transform.position) <= 2.07)
            {
                canPossess = true;
                objToPossess = obj;
                break;
            }
        }

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
                if(canPossess) { Possess(); }
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
            if (npc != null) // Component varsa �al��t�r
            {
                npc.Action();
            }
        }
    }

    private void Possess()
    {
        
    }
}
