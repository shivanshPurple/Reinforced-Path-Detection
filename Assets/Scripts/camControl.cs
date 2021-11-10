using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camControl : MonoBehaviour
{
    public float speed;
    public Transform camPos, car;
    public bool Sim = false;

    private Camera cam;
    private float forward, turn;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (!Sim)
        {
            forward = Input.GetAxis("Vertical");
            turn = Input.GetAxis("Horizontal");

            transform.position += forward * Vector3.forward * speed;
            transform.position += turn * Vector3.right * speed;
        }
    }

    void FixedUpdate()
    {
        if (Sim)
        {
            //transform.rotation = Quaternion.Euler(20, 0, 0);
            Vector3 DesiredPos = camPos.position;
            Vector3 SmoothPos = Vector3.Lerp(transform.position, DesiredPos, speed);
            transform.position = SmoothPos;

            Quaternion DesiredRot = camPos.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, DesiredRot, speed);
        }
    }
}
