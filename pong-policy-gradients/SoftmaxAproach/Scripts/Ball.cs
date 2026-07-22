using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public float startVelocity;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation ;
        rb.gravityScale = 0;

        Setup();
    }

    // Update is called once per frame
    void Setup ()
    {
        transform.position = Vector2.zero;

        var velX = Random.Range(-1, 2);
        if(velX == 0)
            velX = 1;
        var velY = Random.Range(-1, 2);
        if(velY == 0)
            velY = 1;

        rb.velocity = new Vector2(velX, velY) * startVelocity; 	
	}

}
