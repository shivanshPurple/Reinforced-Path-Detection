using UnityEngine;

public class DNA : MonoBehaviour
{
    public float distMult, avgSpeedMult, maxSpeedMult, fitness, reversePenalty = 0.05f;
    public float timeSinceCollision, timeSinceStart, totalDist, lapTime = 0, oldLapTime;
    public int lapCount, id;
    public string member = "random";

    private Vector3 lastPos, startPos, startRot;
    private Rigidbody rb;
    private float maxSpeed;
    private carControl car;
    private bool collision = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        lastPos = transform.position;
        startRot = transform.eulerAngles;
        car = GetComponent<carControl>();
    }

    public void collided()
    {
        car.collided = true;
        collision = true;
        GetComponent<neuralN>().collided = true;
    }

    private void calcFitness()
    {
        float dist = totalDist * distMult;
        float avgSpeed = totalDist / timeSinceStart;

        if (rb.velocity.magnitude>maxSpeed)
        {
            maxSpeed = rb.velocity.magnitude * maxSpeedMult;
        }

        fitness = (dist + avgSpeed + maxSpeed);
        if (car.rpm / Mathf.Abs(car.rpm)<0)
        {
            fitness -= reversePenalty;
        }
    }

    public void reset()
    {
        transform.position = startPos;
        timeSinceStart = 0;
        totalDist = 0;
        maxSpeed = 0;
        fitness = 0;
        car.throttle = 0;
        car.turn = 0;
        rb.velocity = Vector3.zero;
        transform.eulerAngles = startRot;
        car.StartCoroutine("resetRPM");
    }

    private void FixedUpdate()
    {
        //total Distance calculator
        totalDist += Vector3.Magnitude(transform.position - lastPos);
        lastPos = transform.position;

        //time calculator
        timeSinceStart += Time.deltaTime;
        if (collision)
            timeSinceCollision += Time.deltaTime;

        if(!collision)
            calcFitness();

        if((timeSinceStart>20)&&(fitness<10))
            collided();
    }
}
