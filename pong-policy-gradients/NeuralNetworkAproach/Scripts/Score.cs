using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Score : MonoBehaviour
{
    public Action onScore;
    public Action onAntiScore;

    public int score1 = 0;
    public int score2 = 0;
    public float relation;
    
    public void Score1()
    {
        score1 ++;
        onScore();

        relation = (float)score1 / (score1 + score2);
    }
    public void Score2()
    {
        score2 ++;
        onAntiScore();

        relation = (float)score1 / (score1 + score2);
    }
}
