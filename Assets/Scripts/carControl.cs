using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carControl : MonoBehaviour
{
    public WheelCollider[] throttleWheels;
    public WheelCollider[] turnWheels;
    public float rpm, rot;
    public bool collided = false, simStarted;

    public float acceleration = 200;
    public float turnAngle = 20;
    public float brake = 2.5f, velocity;

    [Range(-1f,1f)]
    public float throttle, turn;

    //private float oldTurn = 0;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //velocity = rb.velocity.sqrMagnitude;
        throttle = Input.GetAxis("Vertical");
        turn = Input.GetAxis("Horizontal");
        if (throttle > 1)
            throttle = 1;
        if (throttle < -1)
            throttle = -1;
        if (turn > 1)
            turn = 1;
        if (turn < -1)
            turn = -1;
        if (collided)
        {
            turn = 0;
            throttle = 0;
            resetRPM();
        }
        //slow turning of wheels
        //oldTurn = Mathf.Lerp(oldTurn, turn, 0.01f);
    }

    void FixedUpdate()
    {
        if(!collided)
        {
            foreach (WheelCollider wheel in throttleWheels)
            {
                //applying torque to wheels
                if (Mathf.RoundToInt(wheel.rpm) >= 10 && throttle < 0)
                {
                    throttle = -brake;
                    wheel.motorTorque = acceleration * Time.deltaTime * throttle;
                }

                //else if (Mathf.RoundToInt(wheel.rpm) < 10 && throttle < 0)
                //{
                //    throttle = 0;
                //    wheel.motorTorque = 0;
                //}
                else { wheel.motorTorque = acceleration * Time.deltaTime * throttle; }

                //clamping torque to 200
                if (wheel.motorTorque > 200)
                {
                    wheel.motorTorque = 200;
                }

                //applying torque
                wheel.transform.GetChild(0).GetComponent<Transform>().Rotate(wheel.rpm / 60 * 360 * Time.deltaTime, 0, 0);
                rpm = wheel.rpm;
            }

            foreach (WheelCollider wheel in turnWheels)
            {
                //applying turn to front wheels 
                wheel.steerAngle = turnAngle * turn;

                //applying rotation to front wheels
                wheel.transform.localEulerAngles = new Vector3(0, turnAngle * turn, 0);

                rot = wheel.steerAngle;
            }
        }
    }

    void resetRPM()
    {
        foreach (WheelCollider wheel in throttleWheels)
        {
            wheel.brakeTorque = Mathf.Infinity;
        }
    }
}
