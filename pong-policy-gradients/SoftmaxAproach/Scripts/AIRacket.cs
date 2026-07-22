using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIRacket : MonoBehaviour
{
    public Transform ball;
    public float velocity;

    Rigidbody2D rb;

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.gravityScale = 0;
    }

    void Update()
    {
        
        var dir = ball.transform.position.x - transform.position.x;
        dir = Mathf.Sign(dir);
        dir *= velocity;

        rb.velocity = new Vector3(dir,0,0);
        
	}
}
