using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
using DLFramework;
using DLFramework.Layers;
using DLFramework.Optimizers;
using DLFramework.Layers.Activation;
using DLFramework.Layers.Loss;


public class CartPoleAI : MonoBehaviour
{
    public int getFirst = 7;
    public int samples = 20;
    public int fittingEpoch = 20;
    public float initialTemperature = 1f;
    public float temperatureDecay = 0.05f;
    public double learningRate = 0.01f;
    public int epochCount;
    public int seed = 2;

    private float temperature = 1f;
    private int samplesPlayed = 0;
    private DirectMovementCartPole cartPole;
    private Sequential seq;
    private StochasticGradientDescent sgd;
    private MeanSquaredError mse;
    private System.Random r;

    private List<List<Matrix>> totalInputList = new List<List<Matrix>>();
    private List<List<Matrix>> totalOutputList = new List<List<Matrix>>();

    private List<Matrix> currentInputList = new List<Matrix>();
    private List<Matrix> currentOutputList = new List<Matrix>();

    private void Start()
    {
        r = new System.Random(seed);
        seq = new Sequential();

        seq.Layers.Add(new Linear(4, 100, r));
        seq.Layers.Add(new ReLuLayer());
        seq.Layers.Add(new Linear(100, 10, r));
        seq.Layers.Add(new ReLuLayer());
        seq.Layers.Add(new Linear(10, 1, r));

        sgd = new StochasticGradientDescent(seq.Parameters, learningRate);
        mse = new MeanSquaredError();

        cartPole = GetComponent<DirectMovementCartPole>();
    }

    private void Update()
    {
        // Get prediction
        var AnglePole = cartPole.AnglePole;
        var CartSpeed = cartPole.CartSpeed;
        var PoleAngularSpeed = cartPole.PoleAngularSpeed;
        var CartPosition = cartPole.CartPosition;

        float pred = GetPrediction(AnglePole, CartSpeed, PoleAngularSpeed, CartPosition);
        cartPole.input = pred + Random.Range(-1f, 1f) * temperature;
        cartPole.input = Mathf.Clamp(cartPole.input, -1f, 1f);

        // Save Prediction
        var inputToSave = (Matrix)new double[,] {
            { (double)AnglePole, (double)CartSpeed, (double)PoleAngularSpeed, (double)CartPosition }
        };
        currentInputList.Add(inputToSave);
        var outputToSave = (Matrix)new double[,] {
            { cartPole.input }
        };
        currentOutputList.Add(outputToSave);

        //Reset Enviroment
        if (!cartPole.Reward)
        {
            print("Reward: " + currentInputList.Count);

            cartPole.ResetEnv();
            totalInputList.Add(new List<Matrix>(currentInputList));
            totalOutputList.Add(new List<Matrix>(currentOutputList));

            currentOutputList.Clear();
            currentInputList.Clear();

            samplesPlayed++;

            if (samplesPlayed > samples)
            {
                print("===========================");

                //Decay
                temperature = initialTemperature * Mathf.Exp(-temperatureDecay * epochCount);

                samplesPlayed = 0;
                epochCount++;
                Train();
            }
        }
    }

    private void Train()
    {
        var array = new int[totalInputList.Count];
        for (int i = 0; i < totalInputList.Count; i++)
        {
            array[i] = i;
        }

        System.Array.Sort<int>(array, new System.Comparison<int>(
            (i1, i2) => totalInputList[i2].Count.CompareTo(totalInputList[i1].Count)));

        var newArray = new int[getFirst];
        var regCount = 0;

        for (int i = 0; i < getFirst; i++)
        {
            newArray[i] = i;
            regCount += totalOutputList[i].Count;
        }

        var MatrixX = new double[regCount, 4];
        var MatrixY = new double[regCount, 1];

        var x = 0;

        for (int i = 0; i < newArray.Length; i++)
        {
            for (int j = 0; j < totalInputList[i].Count; j++)
            {
                MatrixX[x, 0] = totalInputList[newArray[i]][j][0, 0];
                MatrixX[x, 1] = totalInputList[newArray[i]][j][0, 1];
                MatrixX[x, 2] = totalInputList[newArray[i]][j][0, 2];
                MatrixX[x, 3] = totalInputList[newArray[i]][j][0, 3];

                MatrixY[x, 0] = totalOutputList[newArray[i]][j][0, 0];

                x++;
            }
        }

        var X = new Tensor(MatrixX, true);
        var Y = new Tensor(MatrixY, true);

        for (var i = 0; i < fittingEpoch; i++)
        {
            var pred = seq.Forward(X);
            var loss = mse.Forward(pred, Y);
            loss.Backward();
            sgd.Step();

            if (double.IsNaN(loss.Data[0, 0]))
            {
                Debug.LogError("LOSS IS NAN");
                Debug.Break();
            }

            print("Epoch: " + i + " Loss: " + loss.Data[0, 0]);
        }

        totalInputList.Clear();
        totalOutputList.Clear();

    }

    private float GetPrediction(float AnglePole, float CartSpeed, float PoleAngularSpeed, float CartPosition)
    {
        var data = new double[,] { { (double)AnglePole, (double)CartSpeed,
            (double)PoleAngularSpeed, (double)CartPosition } };
        var tensor = new Tensor((Matrix)data);
        var resp = seq.Forward(tensor);
        return (float)resp.Data[0, 0];
    }

}
