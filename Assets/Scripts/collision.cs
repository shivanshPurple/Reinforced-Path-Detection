using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    public DNA dna;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wall"))
            dna.collided();

        if ((other.CompareTag("end")) && (dna.totalDist > 150))
        {
            dna.lapTime = dna.timeSinceStart - dna.oldLapTime;
            dna.oldLapTime = dna.lapTime;
        }
    }
}
