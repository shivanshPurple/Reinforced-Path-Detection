using SimpleJSON;
using UnityEngine;
using System.IO;

public class Tester : MonoBehaviour
{
    public GameObject car, cam;
    public int inNum = 4, hidNum = 5, outNum = 2;
    public Vector3 spawnPos;

    private GameObject vehicle;
    void Start()
    {
        vehicle = Instantiate(car, spawnPos, Quaternion.Euler(0, -90, 0));
        copyBrain();
    }

    void copyBrain()
    {
        string path = "C:/temp/finalBrain.json";
        JSONObject data = (JSONObject)JSON.Parse(File.ReadAllText(path));

        neuralN nn = vehicle.GetComponent<neuralN>();
        nn.initNN(4, 5, 2);

        for (int i = 0; i < inNum * hidNum; i++)
        {
            nn.weights1[i] = data["weights1"].AsArray[i];
        }

        for (int i = 0; i < outNum * hidNum; i++)
        {
            nn.weights2[i] = data["weights2"].AsArray[i];
        }
        //vehicle.transform.localScale = scaleCar * Vector3.one;
        nn.collided = false;
        cam.GetComponent<camControl>().enabled = false;
        cam.GetComponent<CamMove>().enabled = true;
        cam.GetComponent<CamMove>().Loc = vehicle.transform.GetChild(5);
    }

}