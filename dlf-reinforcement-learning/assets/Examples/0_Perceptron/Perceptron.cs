using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;
using DLFramework;
using DLFramework.Layers;
using DLFramework.Optimizers;
using DLFramework.Layers.Activation;
using DLFramework.Layers.Loss;
using System.IO;

public class Perceptron : MonoBehaviour
{


    // Start is called before the first frame update
    IEnumerator Start()
    {
        var r = new System.Random(2);

        var x = (Matrix)new double[1000, 1];
        Matrix.MatrixLoop((i, j) =>
        {
            x[i, 0] = i;
        }, x.X, x.Y);

        var y = (Matrix)new double[1000, 1];
        Matrix.MatrixLoop((i, j) =>
        {
            y[i, 0] = i * 12 + 15 + r.Next(10);
        }, x.X, x.Y);

        // var x = new double[,] { { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } };
        // var y = new double[,] { { 0 }, { 1 }, { 0 }, { 1 } };

        var X = new Tensor(x, true);
        var Y = new Tensor(y, true);

        var seq = new Sequential();
        seq.Layers.Add(new Linear(1, 1, r));

        var sgd = new StochasticGradientDescent(seq.Parameters, 0.001);

        var mse = new MeanSquaredError();

        for (var i = 0; i < 10000; i++)
        {
            yield return null;
            var pred = seq.Forward(X);
            print(pred.Data.Size);
            var loss = mse.Forward(pred, Y);

            loss.Backward();
            sgd.Step();
            print($"Epoch: {i} Loss: {loss.Data[0, 0]}");
            print(Y);
            print(pred);
        }

        print(seq.Forward(new Tensor(x)));
    }

    static void sigmoid(Matrix m, bool derivated = false)
    {

        /*
        w = list()
        w.append(Tensor(np.random.rand(2,3), autograd=True))
        w.append(Tensor(np.random.rand(3,1), autograd=True))
        for i in range(10):
         pred = data.mm(w[0]).mm(w[1])

         loss = ((pred - target)*(pred - target)).sum(0)

         loss.backward(Tensor(np.ones_like(loss.data)))
         for w_ in w:
         w_.data -= w_.grad.data * 0.1
         w_.grad.data *= 

        */



    }

    static Matrix relu(Matrix m, bool derivated = false)
    {
        double[,] a = m;
        Matrix.MatrixLoop((i, j) =>
        {
            if (derivated)
            {
                a[i, j] = a[i, j] > 0 ? 1 : 0.00001;
            }
            else
            {
                a[i, j] = a[i, j] > 0 ? a[i, j] : 0;
            }
        }, m.X, m.Y);

        return a;
    }

    private Matrix LoadData()
    {
        List<string[]> d = new List<string[]>();
        using (var reader = new StreamReader(@"Assets/Examples/0_Perceptron/mnist_train.csv"))
        {

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                d.Add(values);
            }
        }

        var dataLoaded = new double[d.Count, d[0].Length];

        Matrix.MatrixLoop((i, j) =>
        {
            dataLoaded[i, j] = double.Parse(d[i][j]);
        }, dataLoaded.GetLength(0), dataLoaded.GetLength(1));

        return (Matrix)dataLoaded;
    }
}
