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
    
    public void Score1()
    {
        score1 ++;
        onScore();
    }
    public void Score2()
    {
        score2 ++;
        onAntiScore();
    }
}
