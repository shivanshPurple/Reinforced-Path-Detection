using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using System.Collections.Generic;

public class lvlDesign : MonoBehaviour
{

    public GameObject hexagon, car, cam, plane, pole;
    public Transform env;
    public int x_hex = 8, y_hex = 8, inNum = 4, hidNum = 5, outNum = 2;
    public bool RoadsFinished = false;
    public Button switch1;
    public float scaleCar = 0.005f;

    private int count = 0;
    private bool CarSpawned = false;
    private GameObject vehicle = null;
    void Start()
    {
        for (int i = 0; i < y_hex; i++)
        {
            for (int j = 0; j < x_hex; j++)
            {
                Vector3 x_pos = new Vector3(3.45f * i, 0, 5.8f * j);
                GameObject hex = Instantiate(hexagon, x_pos, Quaternion.Euler(-90, 0, 0), env);
                hex.GetComponent<hexControl>().id = count;
                //if (count == 28 || count == 35 || count == 36 || count == 37 || count == 42 || count == 43 || count == 44 || count == 49 || count == 50 || count == 51 || count == 56)
                //hex.GetComponent<hexControl>().road = true;
                count++;
            }

            for (int k = 0; k < x_hex; k++)
            {
                Vector3 pos = new Vector3((3.45f * i) + (1.72f), 0, 5.8f * k + 2.9f);
                GameObject hex = Instantiate(hexagon, pos, Quaternion.Euler(-90, 0, 0), env);
                hex.GetComponent<hexControl>().id = count;
                //if (count == 28 || count == 35 || count == 36 || count == 37 || count == 42 || count == 43 || count == 44 || count == 49 || count == 50 || count == 51 || count == 56)
                    //hex.GetComponent<hexControl>().road = true;
                count++;
            }
        }

        //float[] roads = {28, 35, 36, 37, 42, 43, 44, 49, 50, 51, 56};
        //foreach (float road in roads)
        //{

        //}
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && RoadsFinished && !CarSpawned)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.GetComponent<hexControl>().road)
                {
                    vehicle = Instantiate(car, hit.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                    vehicle.GetComponent<neuralN>().collided = true;
                    plane.transform.position += new Vector3(0, 0.75f, 0);
                    Instantiate(pole, hit.transform.position, Quaternion.Euler(0, 0, 0));
                    CarSpawned = true;
                    copyBrain();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void FinishRoad()
    {
        RoadsFinished = true;
        switch1.gameObject.SetActive(false);
    }

    void copyBrain()
    {
        string path = "C:/temp/finalBrain.json";
        JSONObject data = (JSONObject) JSON.Parse(File.ReadAllText(path));

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
        vehicle.transform.localScale = scaleCar * Vector3.one;
        nn.collided = false;
        cam.GetComponent<camControl>().enabled = false;
        cam.GetComponent<CamMove>().enabled = true;
        cam.GetComponent<CamMove>().Loc = vehicle.transform.GetChild(5);
    }

    public void StartSim()
    {
        car.GetComponent<carControl>().simStarted = true;
        cam.GetComponent<camControl>().camPos = vehicle.transform.GetChild(5);
        cam.GetComponent<camControl>().car = vehicle.transform;
        cam.GetComponent<camControl>().Sim = true;
    }
}
