using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearAlgebra;

[RequireComponent(typeof(Rigidbody2D))]
public class PolicyAgent : MonoBehaviour
{
    public float timeMultiplier = 2;
    public double learningRate = 0.0025f;
    public int epoch = 2000;
    public float gamma = 0.99f;
    public float velocity;
    public Rigidbody2D ball;
    public Transform otherRacket;
    public Score score;

    Rigidbody2D rb;
    Matrix w;

    Matrix Policy(Matrix state)
    {
        var z = state * w;
        var exp = Matrix.Exp(z);
        exp = exp / Matrix.Sumatory(exp)[0,0];
        return exp;
    }

    Matrix GradientSoftmax(Matrix softmax){
        Matrix shape = softmax.T;
        Matrix diagoFlat = Matrix.DiagFlat(shape);
        diagoFlat = diagoFlat - shape * shape.T;
        return diagoFlat;
    }

    System.Random r;
    double reward;
    // Start is called before the first frame update
    void Start()
    {
        r = new System.Random(1);
        w = Matrix.Random(6,3, r);

         Time.timeScale = timeMultiplier;

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.gravityScale = 0;

        score.onScore = ()=>{reward++;};
        score.onAntiScore = ()=>{reward-= 1.5;};
        
    }


    List<Matrix> gradHistory = new List<Matrix>();

    // Update is called once per frame
    void Update()
    {
        Matrix envi = new double[,]{
            {transform.position.x,
            otherRacket.position.x,
            ball.transform.position.x,
            ball.transform.position.y,
            ball.velocity.x,
            ball.velocity.y}};

        var action = Policy(envi);

        var index = Matrix.RandomChoice(action, r);

        var dir = 0;

        if(index == 0){
            dir = -1;
        } 
        else if( index == 1)
        {
            dir = 0;
        } 
        else if(index == 2){
            dir = 1;
        }

        rb.velocity = new Vector3(dir * velocity, 0);	        

        //GUARDAR LOS GRADIENTES
        var dsoftmax = GradientSoftmax(action);
        dsoftmax = dsoftmax.Slice(index,0,index+1,dsoftmax.X);
        dsoftmax = dsoftmax / action[0, index];
        var grad = envi.T * dsoftmax;

        gradHistory.Add(grad);
        
        if(reward != 0){
            Train(reward);
            reward = 0;
        }
    }

    double decayRatio(int x, double reward){
        double sum = 0;

        for(int i = 0; i < x ; i++){

            sum += reward * (System.Math.Pow(gamma, reward));

        }

        return sum;
    }

    void Train(double reward)
    {
        for(int i = 0 ; i < gradHistory.Count; i++)
        {
            var update = gradHistory[i] * (learningRate * 
                decayRatio(i, reward));

            w += update;
        }
        gradHistory = new List<Matrix>();
    }

}
