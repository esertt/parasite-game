using System;
using UnityEngine;

public class NPCLogic : MonoBehaviour
{
    public float speed;
    public int[] movement;
    public Transform movePoint;

    private int arraySize;
    private int currentMovement;
    void Start()
    {
        speed = 5f;
        movePoint.parent = null;
        arraySize = movement.Length;
        currentMovement = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.tag == "baby" || gameObject.tag == "scientist")
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
     * 1 clockward 2 anti clockward
     */
    public void Action()
    {
        if(gameObject.tag == "scientist" || gameObject.tag == "baby")
        {
            switch (movement[currentMovement])
            {
                case 1:
                    movePoint.position += new Vector3(0, 1, 0);
                    Debug.Log(1);
                    break;
                case 2:
                    movePoint.position += new Vector3(1, 0, 0);
                    Debug.Log(2);
                    break;
                case 3:
                    movePoint.position += new Vector3(0, -1, 0);
                    Debug.Log(3);
                    break;
                case 4:
                    movePoint.position += new Vector3(-1, 0, 0);
                    Debug.Log(4);
                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }else if(gameObject.tag == "soldier")
        {

        }
        currentMovement = (currentMovement + 1) % arraySize;
    }
}
