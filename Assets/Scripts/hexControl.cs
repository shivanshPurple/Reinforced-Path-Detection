using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hexControl : MonoBehaviour
{
    public Material mat_road, mat_plane;
    public GameObject manager;
    public bool road = false;
    public int id;
    public bool RoadFinished = false;

    private float original_y;

    void Start()
    {
        original_y = transform.position.y;
        manager = GameObject.FindGameObjectWithTag("agent");
    }

    void Update()
    {
        RoadFinished = manager.GetComponent<lvlDesign>().RoadsFinished;
    }

    void OnMouseDown()
    {
        if (!RoadFinished)
        {
            //if mouseclick on hexagon is not on already enabled road
            if (transform.position.y == original_y)
            {
                //change position
                Vector3 newpos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
                transform.position = newpos;

                //change color
                GetComponent<Renderer>().material = mat_road;

                //change id of hexagon
                road = true;
            }

            //if mouseclick on hexagon is on already enabled road
            else if (transform.position.y + 1 == original_y)
            {
                //change position
                Vector3 newpos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                transform.position = newpos;

                //change color
                GetComponent<Renderer>().material = mat_plane;

                //change id of hexagon
                road = false;
            }
        }
    }
}
