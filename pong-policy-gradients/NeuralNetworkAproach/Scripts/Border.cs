using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour {

    public enum Team{
        team1, team2
    }

    public Team team;

    public Score score;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ball"))
        {
            col.SendMessage("Setup");
            if(team == Team.team1)
            {
                score.Score1();
            }
            else
            {
                score.Score2();
            }
        }
    }
}
