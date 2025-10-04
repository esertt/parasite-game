using System;
using UnityEngine;

public class NPCLogic : MonoBehaviour
{
    public float speed;
    public int[] movement;
    public Transform movePoint;
    public Sprite up;
    public Sprite down;

    //soldier special variables


    private int arraySize;
    private int currentMovement;
    private SpriteRenderer sr;
    void Start()
    {
        speed = 5f;
        movePoint.parent = null;
        arraySize = movement.Length;
        currentMovement = 0;
        sr = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "baby" || gameObject.tag == "scientist")
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, movePoint.position, speed * Time.deltaTime);
        }
    }


    /*
     * for baby and scientist
     * 1 up 3 down
     * 2 right 4 left
     * 
     * for soldier 
     * same thing but instead of movement direction of their gun changes
     */
    public void Action()
    {
        if (gameObject.tag == "scientist" || gameObject.tag == "baby")
        {
            switch (movement[currentMovement])
            {
                case 1:
                    movePoint.position += new Vector3(0, 1, 0);
                    sr.sprite = up;
                    break;
                case 2:
                    movePoint.position += new Vector3(1, 0, 0);
                    sr.sprite = down;
                    break;
                case 3:
                    movePoint.position += new Vector3(0, -1, 0);
                    sr.sprite = down;
                    break;
                case 4:
                    movePoint.position += new Vector3(-1, 0, 0);
                    sr.sprite = down;
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        } else if (gameObject.tag == "soldier")
        {
            switch (movement[currentMovement])
            {
                case 1:
                    sr.sprite = up;
                    break;
                case 2:
                    sr.sprite = down;
                    break;
                case 3:
                    sr.sprite = down;
                    break;
                case 4:
                    sr.sprite = down;
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
        currentMovement = (currentMovement + 1) % arraySize;

    }
}