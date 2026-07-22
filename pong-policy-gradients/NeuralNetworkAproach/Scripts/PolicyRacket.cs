using System.Collections;
using System.Collections.Generic;
using LinearAlgebra;
using UnityEngine;

public class PolicyRacket : PolicyAgent {

    [Header ("RacketRelated")]
    public float velocity;
    public Rigidbody2D ball;
    public Transform otherRacket;
    public Score score;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start () {
        SetUp ();

        rb = GetComponent<Rigidbody2D> ();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.gravityScale = 0;

        score.onScore = () => {
            reward += 1;
        };

        score.onAntiScore = () => {
            reward -= 5;
        };

    }

    // Update is called once per frame
    void Update () {

        Matrix envi = new double[, ] {
            {
            transform.position.x,
            otherRacket.position.x,
            ball.transform.position.x,
            ball.transform.position.y,
            ball.velocity.x,
            ball.velocity.y
            }
        };

        //Get the prediction
        var action = GetPrediction (envi);

        //
        rb.velocity = new Vector3 ((float) action[0, 0] * velocity, 0);
    }

    protected override Matrix ClearAction (Matrix action) {
        //Clamps action between -1, 1
        action[0, 0] = action[0, 0] > 1 ? 1 : action[0, 0] < -1 ? -1 : action[0, 0];
        return action;
    }
}