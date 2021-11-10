using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class geneticAlgo : MonoBehaviour
{
    public GameObject car;
    public int populationPerGen = 20, childrenPerGen = 40, gen = 0;
    public float mutationRate = 0.1f;
    public List<GameObject> children;
    public CamMove cam;
    public Material green;
    public bool topCam = false;
    public TextMeshProUGUI genText, popText, mutText, bestGenome, bestThrottle, bestTurn, bestFitness, bestLap;
    public Slider slider;

    private int currentPop, inNum = 4, hidNum = 5, outNum = 2;
    private Transform bestCar, secondBest;
    private bool onHold = false;

    void Start()
    {
        createGen();
        popText.text = "Population - " + populationPerGen;
        mutText.text = "Mutation Rate - " + mutationRate;
    }

    public void changeTimeScale()
    {
        Time.timeScale = slider.value;
    }

    public void generation()
    {
        saveDataOfGen(gen);
        killGen();
        createGen();
        loadChildren();
    }

    public void changeCam()
    {
        if (topCam)
            topCam = false;
        else
            topCam = true;
    }

    void saveDataOfGen(int gen)
    {
        JSONObject data = new JSONObject();
        data.Add("generation", gen);

        //neural net of best car
        neuralN bestNN = bestCar.GetComponent<neuralN>();
        JSONArray weights1 = new JSONArray();
        JSONArray weights2 = new JSONArray();
        for (int i = 0; i < bestNN.hidNum * bestNN.inNum; i++)
        {
            weights1.Add(bestCar.GetComponent<neuralN>().weights1[i]);
        }

        for (int i = 0; i < bestNN.hidNum * bestNN.outNum; i++)
        {
            weights2.Add(bestCar.GetComponent<neuralN>().weights2[i]);
        }

        data.Add("weights1", weights1);
        data.Add("weights2", weights2);
        data.Add("fitness1", bestCar.GetComponent<DNA>().fitness);

        //neural net of second best car
        neuralN secondNN = secondBest.GetComponent<neuralN>();
        JSONArray weights3 = new JSONArray();
        JSONArray weights4 = new JSONArray();
        for (int i = 0; i < secondNN.hidNum * secondNN.inNum; i++)
        {
            weights3.Add(secondBest.GetComponent<neuralN>().weights1[i]);
        }

        for (int i = 0; i < secondNN.hidNum * secondNN.outNum; i++)
        {
            weights4.Add(secondBest.GetComponent<neuralN>().weights2[i]);
        }

        data.Add("weights3", weights3);
        data.Add("weights4", weights4);
        data.Add("fitness2", secondBest.GetComponent<DNA>().fitness);

        //saving data to computer in json file
        string temp = "/generation" + gen + ".json";
        string path = "C:/temp" + temp;
        File.WriteAllText(path, data.ToString());
    }

    void loadChildren()
    {
        string temp = "/generation" + (gen-1) + ".json";
        string path = "C:/temp" + temp;
        string str = File.ReadAllText(path);
        JSONObject data = (JSONObject) JSON.Parse(str);
        float fitness1 = data["fitness1"];
        float fitness2 = data["fitness2"];
        float[] weights1 = new float[hidNum * inNum];
        float[] weights2 = new float[hidNum * outNum];
        float[] weights3 = new float[hidNum * inNum];
        float[] weights4 = new float[hidNum * outNum];

        for (int i = 0; i < hidNum * inNum; i++)
        {
            weights1[i] = data["weights1"].AsArray[i];
        }

        for (int i = 0; i < hidNum * outNum; i++)
        {
            weights2[i] = data["weights2"].AsArray[i];
        }

        for (int i = 0; i < hidNum * inNum; i++)
        {
            weights3[i] = data["weights3"].AsArray[i];
        }

        for (int i = 0; i < hidNum * outNum; i++)
        {
            weights4[i] = data["weights4"].AsArray[i];
        }

        cloneParents(weights1, weights2, weights3, weights4);
        crossoverFromParent(fitness1, fitness2, weights1, weights2, weights3, weights4);
        mutateChildren();

        onHold = false;
    }

    void mutateChildren()
    {
        for (int i = 2; i < childrenPerGen; i++)
        {
            neuralN nn = children[i].GetComponent<neuralN>();
            for (int j = 0; j < hidNum * inNum; j++)
            {
                float random = Random.value;
                if (random <= mutationRate)
                {
                    nn.weights1[j] = Random.Range(0f, 1f);
                }
            }

            for (int j = 0; j < outNum * hidNum; j++)
            {
                float random = Random.value;

                if (random <= mutationRate)
                {
                    nn.weights2[j] = Random.Range(0f, 1f);
                }
            }
        }
    }

    void cloneParents(float[] weights1, float[] weights2, float[] weights3, float[] weights4)
    {
        neuralN nn = children[0].GetComponent<neuralN>();
        nn.weights1 = new List<float>(weights1);
        nn.weights2 = new List<float>(weights2);
        nn.GetComponent<DNA>().member = "parent";
        nn = children[1].GetComponent<neuralN>();
        nn.weights1 = new List<float>(weights3);
        nn.weights2 = new List<float>(weights4);
        nn.GetComponent<DNA>().member = "parent";
        for (int i = 0; i < 2; i++)
        {
            children[i].transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.black);
            children[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
            children[i].transform.GetChild(4).GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
        }
    }

    void crossoverFromParent(float fitness1, float fitness2, float[] weights1, float[] weights2, 
        float[] weights3, float[] weights4)
    {
        float totalFitness = fitness1 + fitness2;
        for (int i = 2; i < childrenPerGen; i++)
        {
            neuralN nn = children[i].GetComponent<neuralN>();
            nn.weights1.Clear();
            nn.weights2.Clear();
            for (int j = 0; j < hidNum * inNum; j++)
            {
                float random = Random.Range(0, totalFitness);
                if (random > fitness1)
                    nn.weights1.Add(weights3[j]);
                else
                    nn.weights1.Add(weights1[j]);
            }

            for (int j = 0; j < outNum * hidNum; j++)
            {
                float random = Random.Range(0, totalFitness);

                if (random > fitness1)
                    nn.weights2.Add(weights2[j]);
                else
                    nn.weights2.Add(weights4[j]);
            }
            children[i].GetComponent<DNA>().member = "child";
        }
    }

    void killGen()
    {
        foreach (GameObject car in children)
        {
            Destroy(car);
            currentPop--;
        }
        bestLap.text = "Best Lap - NA";
        onHold = true;
    }

    void createGen()
    {
        if (currentPop == 0)
        {
            children = new List<GameObject>(populationPerGen);
            for (int i = 0; i < populationPerGen; i++)
            {
                GameObject c = Instantiate(car);
                children.Add(c);
                currentPop++;
                c.GetComponent<neuralN>().initNN(inNum, hidNum, outNum);
                c.GetComponent<DNA>().id = i+1;
            }
            gen++;
            genText.text = "Generation - " + gen;
            cam.Loc = children[0].transform;
            bestCar = children[0].transform;
            secondBest = children[1].transform;
        }
    }

    void Update()
    {
        if (!onHold)
            findBest();
        otherChecks();
        if (topCam)
            cam.Loc = bestCar.GetChild(7);
        else
            cam.Loc = bestCar.GetChild(5);
        bestGenome.text = "Genome ID - " + bestCar.GetComponent<DNA>().id.ToString();
        bestFitness.text = "Fitness - " + bestCar.GetComponent<DNA>().fitness.ToString("0.00");
        bestThrottle.text = "Throttle - " + bestCar.GetComponent<carControl>().throttle.ToString("0.00");
        bestTurn.text = "Turn - " + bestCar.GetComponent<carControl>().turn.ToString("0.00");
    }

    void otherChecks()
    {
        for (int i = 0; i < populationPerGen; i++)
        {
            if (!children[i].GetComponent<carControl>().collided)
                break;
            if ((children[i].GetComponent<carControl>().collided) && (i == populationPerGen - 1))
            {
                generation();

                //Debug.Log("All cars have collided so next gen is automatically created");
            }
        }
        DNA bestdna = bestCar.GetComponent<DNA>();
        if ((bestCar.GetComponent<carControl>().collided) && (bestdna.timeSinceCollision > 10))
            generation();

        if (bestdna.timeSinceStart > 300)
            generation();

        if ((bestdna.lapTime != 0))
        {
            bestLap.text = "Best Lap" + bestdna.lapTime.ToString("0.00");
            bestdna.oldLapTime = bestdna.lapTime;
            bestdna.lapTime = 0;
            bestdna.lapCount++;
        }

        if (bestLap.text != "Best Lap - NA")
        {
            if(bestdna.oldLapTime<bestdna.lapTime)
            {
                bestLap.text = "Best Lap" + bestdna.lapTime.ToString("0.00");
                bestdna.oldLapTime = bestdna.lapTime;
                bestdna.lapTime = 0;
                bestdna.lapCount++;
            }
        }

    }

    void findBest()
    {
        foreach(GameObject car in children)
        {
            float sample = car.GetComponent<DNA>().fitness;
            float best = bestCar.GetComponent<DNA>().fitness;
            float second = secondBest.GetComponent<DNA>().fitness;
            MeshRenderer carMesh = car.transform.GetChild(6).GetComponent<MeshRenderer>();
            if (sample > best)
            {
                if (sample == second)
                {
                    secondBest.GetChild(6).GetComponent<MeshRenderer>().enabled = false;
                    if (carMesh.enabled == false)
                    {
                        carMesh.enabled = true;
                    }
                    secondBest = bestCar;
                    bestCar = car.transform;
                    bestCar.GetChild(6).GetComponent<MeshRenderer>().material = green;
                    secondBest.GetChild(6).GetComponent<MeshRenderer>().material = green;
                }
                else
                {
                    bestCar.GetChild(6).GetComponent<MeshRenderer>().enabled = false;
                    if (carMesh.enabled == false)
                    {
                        carMesh.enabled = true;
                    }
                    bestCar = car.transform;
                    cam.Loc = bestCar.GetChild(5);
                    bestCar.GetChild(6).GetComponent<MeshRenderer>().material = green;
                }
            }

            else if ((sample > second) && (sample < best) && (sample != best))
            {
                secondBest.GetChild(6).GetComponent<MeshRenderer>().enabled = false;
                if (carMesh.enabled == false)
                {
                    carMesh.enabled = true;
                }
                secondBest = car.transform;
                secondBest.GetChild(6).GetComponent<MeshRenderer>().material = green;

            }
        }
    }

    public void stopAndTest()
    {
        string path = "C:/temp/finalBrain.json";
        JSONObject brain = new JSONObject();
        neuralN nn = bestCar.GetComponent<neuralN>();
        JSONArray weights1 = new JSONArray();
        JSONArray weights2 = new JSONArray();
        for (int i = 0; i < hidNum * inNum;i++)
            weights1.Add(nn.weights1[i]);
        for (int i = 0; i < hidNum * outNum; i++)
            weights2.Add(nn.weights2[i]);

        brain.Add("weights1", weights1);
        brain.Add("weights2", weights2);

        File.WriteAllText(path,brain.ToString());

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}