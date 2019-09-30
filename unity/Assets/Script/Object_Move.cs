using UnityEngine;

using System.Collections;



public class Object_Move : MonoBehaviour
{

    public Vector3 Move_Point;

    public float Move_Speed = 5;

    // Use this for initialization

    void Start()
    {

        GetComponent<Renderer>().material.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);



    }



    // Update is called once per frame

    void Update()
    {

        float distance = Vector3.Distance(transform.position, Move_Point);

        if (distance != 0)

        {

            transform.position = Vector3.MoveTowards(transform.position, Move_Point, Move_Speed * Time.deltaTime);

        }

    }

}