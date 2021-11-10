using System.Collections.Generic;
using UnityEngine;

public class neuralN : MonoBehaviour
{   
    public List<float> weights1, weights2, inputL, hiddenL, outputL;
    public int inNum, hidNum, outNum;
    public Rays[] demRays;
    public bool collided = false;

    private carControl car;
    
    //just initializes neural net with random values
    public void initNN(int inputNum, int hiddenNum,int outputNum)
    {
        inNum = inputNum;
        hidNum = hiddenNum;
        outNum = outputNum;

        weights1 = new List<float>(inNum * hidNum);
        weights2 = new List<float>(outNum * hidNum);
        inputL = new List<float>(inNum);
        hiddenL = new List<float>(hidNum);
        outputL = new List<float>(outNum);

        for (int i = 0;i < hiddenNum * inputNum; i++)
        {
            float number = (float)Random.Range(-100, 100);
            number /= 100;
            weights1.Add(number);
        }

        for (int i = 0; i < hiddenNum * outputNum; i++)
        {
            float number = (float)Random.Range(-100, 100);
            number /= 100;
            weights2.Add(number);
        }

        //add the input values
        inputL = fetchInputs();

        hiddenL = calcHLayer(inputL, weights1);
        outputL = calcOLayer(hiddenL, weights2);
    }

    private void updateNN()
    {
        inputL = fetchInputs();
        hiddenL = calcHLayer(inputL, weights1);
        outputL = calcOLayer(hiddenL, weights2);
        parseOutputs();
    }

    private void parseOutputs()
    {
        car.throttle = outputL[0];
        car.turn = outputL[1];
    }

    private List<float> fetchInputs()
    {
        List<float> layer = new List<float>();
        foreach (Rays r in demRays)
        {
            layer.Add(r.dist);
        }
        layer.Add(GetComponent<Rigidbody>().velocity.sqrMagnitude/175);
        return layer;
    }

    //function that calculates hidden layer by multiplying inputs and weights1
    public List<float> calcHLayer(List<float> inputL, List<float> weights1)
    {
        List<float> layer = new List<float>();
        int j = 0;
        for (int hiddenLNode = 0; hiddenLNode < hidNum; hiddenLNode++)
        {
            float sum = 0;
            for (int i = 0; i < inNum; i++)
            {
                sum += inputL[i] * weights1[j];
                j++;
            }
            layer.Add(sum);
        }
        return layer;
    }

    //function that calculates hidden layer by multiplying hiddens and weights2
    public List<float> calcOLayer(List<float> hiddenL, List<float> weights2)
    {
        List<float> layer = new List<float>();
        int j = 0;
        for (int outputLNode = 0; outputLNode < outNum; outputLNode++)
        {
            float sum = 0;
            for (int i = 0; i < hidNum; i++)
            {
                sum += hiddenL[i] * weights2[j];
                j++;
            }
            //sum = sigmoid(sum);
            layer.Add(sum);
        }
        return layer;
    }

    void Start()
    {
        car = GetComponent<carControl>();
    }

    void Update()
    {
        if(!collided)
            updateNN();
    }
}
