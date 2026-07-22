using System.Collections;
using System.Collections.Generic;
using LinearAlgebra;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class PolicyAgent : MonoBehaviour {

    protected enum ActivationFunction {
        Sigmoid,
        ReLu
    }

    [Header ("HyperParameters")]
    [SerializeField]
    private ActivationFunction activationFunction = ActivationFunction.ReLu;

    [SerializeField]
    private float timeMultiplier = 2;

    [SerializeField]
    private double learningRate = 0.0025;

    [SerializeField]
    private double gamma = 0.99;

    [SerializeField]
    private int randomSeed = 1;

    [SerializeField]
    private double hotness = 0.1;

    [SerializeField]
    private double hotnessAmplitude = 5;

    [SerializeField]
    private int[] NeuronCount = new int[] { 2, 3, 3, 1 };

    Matrix[] W;
    int LayerCount;
    System.Random r;
    protected double reward;

    protected virtual Matrix ClearAction (Matrix action) => action;

    private void ForwardPropagation (out Matrix[] Z, out Matrix[] A, Matrix InputValue) {
        Z = new Matrix[LayerCount];
        A = new Matrix[LayerCount];

        Z[0] = InputValue.AddColumn (Matrix.Ones (InputValue.Size.X, 1));
        A[0] = Z[0];

        for (int i = 1; i < LayerCount; i++) {
            Z[i] = (A[i - 1] * W[i - 1]).AddColumn (Matrix.Ones (InputValue.Size.X, 1));
            A[i] = Activation (Z[i]);
        }
        A[A.Length - 1] = Z[Z.Length - 1];
    }
    private void BackPropagation (out Matrix[] delta, out Matrix[] error, Matrix output, Matrix expectedOutput,
        Matrix[] Z) {
        error = new Matrix[LayerCount];
        delta = new Matrix[LayerCount];

        error[LayerCount - 1] = expectedOutput - output;
        delta[LayerCount - 1] = error[LayerCount - 1]; // * Relu(Zlast, true);

        for (int i = LayerCount - 2; i >= 0; i--) {
            error[i] = delta[i + 1] * W[i].T;
            delta[i] = error[i] * Activation (Z[i], true);
            delta[i] = delta[i].Slice (0, 1, delta[i].X, delta[i].Y);
        }

    }

    private Matrix Activation (Matrix m, bool derivated = false) {

        switch (activationFunction) {
            case ActivationFunction.ReLu:
                return Relu (m, derivated);
            case ActivationFunction.Sigmoid:
                return Sigmoid (m, derivated);
        }

        return null;
    }

    private Matrix Relu (Matrix m, bool derivated = false) {
        double[, ] output = m;
        Matrix.MatrixLoop ((i, j) => {
            if (derivated) {
                output[i, j] = output[i, j] > 0 ? 1 : 0;
            } else {
                output[i, j] = output[i, j] > 0 ? output[i, j] : 0.0001 * output[i, j];
            }

        }, m.X, m.Y);
        return output;
    }

    private Matrix Sigmoid (Matrix m, bool derivated = false) {
        double[, ] output = m;
        Matrix.MatrixLoop ((i, j) => {
            if (derivated) {
                double aux = 1 / (1 + System.Math.Exp (-output[i, j]));
                output[i, j] = aux * (1 - aux);
            } else {
                output[i, j] = 1 / (1 + System.Math.Exp (-output[i, j]));
            }

        }, m.X, m.Y);
        return output;
    }

    protected void SetUp () {
        LayerCount = NeuronCount.Length;

        W = new Matrix[LayerCount - 1];
        r = new System.Random (randomSeed);

        for (int i = 0; i < W.Length; i++) {
            W[i] = Matrix.Random (NeuronCount[i] + 1, NeuronCount[i + 1], r) * 2 - 1;
        }

        Time.timeScale = timeMultiplier;
    }

    List<Matrix[]> aHistory = new List<Matrix[]> ();
    List<Matrix[]> gradHistory = new List<Matrix[]> ();
    List<double> rewardHistory = new List<double> ();

    // Update is called once per frame
    protected Matrix GetPrediction (Matrix envi) {

        Matrix[] Z, A;

        ForwardPropagation (out Z, out A, envi);

        var output = A[A.Length - 1].Slice (0, 1, A[A.Length - 1].X, A[A.Length - 1].Y);

        Matrix action;

        if (r.NextDouble () <= hotness) {
            var randomness = (Matrix.Random (output.X, output.Y, r) - 0.5) * hotnessAmplitude;
            action = randomness;

            // print (action);
        } else {
            action = output;
            // print ($"<b>{action}</b>");
        }

        // print (action);

        action = ClearAction (action);

        //SAVE GRADIENTS
        Matrix[] error, delta;

        BackPropagation (out delta, out error, output, action, Z);

        gradHistory.Add (delta);
        aHistory.Add (A);
        rewardHistory.Add (reward);

        if (reward != 0) {
            Train ();
            reward = 0;
        }

        return action;

    }

    protected void Train () {
        for (int i = 0; i < gradHistory.Count; i++) {
            double decay = 0;

            for (int k = i; k < rewardHistory.Count; k++) {
                decay += rewardHistory[k] * (System.Math.Pow (gamma, rewardHistory[k]));
            }

            for (int j = 0; j < W.Length; j++) {

                W[j] += (aHistory[i][j].T * gradHistory[i][j + 1]) *
                    (learningRate * -decay);
            }
        }
        rewardHistory = new List<double> ();
        aHistory = new List<Matrix[]> ();
        gradHistory = new List<Matrix[]> ();
    }

}