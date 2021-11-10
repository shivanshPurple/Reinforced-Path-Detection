using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rays : MonoBehaviour
{
    public GameObject parent;
    public Material redMat, greenMat, purpleMat;
    public float dist;
    public Vector3 offset;
    
    private void FixedUpdate()
    {
        Ray r = new Ray(transform.position, transform.up*5);

        RaycastHit hit;
        
        if(Physics.Raycast(r, out hit, 5))
        {
            dist = Vector3.Magnitude(hit.point - r.origin);
            dist /= 5;
        }
        if (dist == 0)
        {
            dist = 1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 2)
        {
            if (other.gameObject.CompareTag("wall"))
            {
                parent.GetComponent<Renderer>().material = redMat;
            }
        }
        if (other.gameObject.CompareTag("fuel"))
        {
            parent.GetComponent<Renderer>().material = greenMat;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("fuel") || other.gameObject.CompareTag("wall"))
            defaultMat();
    }

    public void defaultMat()
    {
        parent.GetComponent<Renderer>().material = purpleMat;
    }
}
