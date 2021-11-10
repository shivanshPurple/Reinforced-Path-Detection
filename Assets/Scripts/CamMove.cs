using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public float smoothness = 0.05f;
    public Transform Loc;

    private Camera cam;
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (Loc!=null)
        {
            Vector3 pos = Vector3.Lerp(cam.transform.position, Loc.transform.position, smoothness);
            cam.transform.position = pos;
            cam.transform.LookAt(Loc.parent, Vector3.up);
        }
    }
}
