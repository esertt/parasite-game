using UnityEngine;
using UnityEngine.Tilemaps;

public class puzzleAction : MonoBehaviour
{
    private bool isInfected;
    private bool actionReady;

    public float speed = 5.0f;
    public Transform movePoint;
    public Tilemap tileMap;
    public GameObject parasite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInfected = false;
        movePoint.parent = null;
        actionReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        parasite.transform.position = Vector3.MoveTowards(parasite.transform.position, movePoint.position, speed*Time.deltaTime);

        if (Vector3.Distance(parasite.transform.position, movePoint.position) <= 0f) actionReady = true;
        else actionReady = false;
        if (actionReady)
        {
            //first update the desire path
            Vector3 newPos = movePoint.position;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1)
            {
                newPos = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
            {
                newPos = movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }

            //check the desired path. If player can move then update the movePosition
            Vector3Int cellPos = tileMap.WorldToCell(newPos);
            TileBase tile = tileMap.GetTile(cellPos);
            if (tile != null && tile.name == "tiletest_1")
            {
                Debug.Log("Tile found at " + cellPos + ": " + tile.name);
            }
            else
            {
                movePoint.position = newPos;
            }
        }
    }
}
