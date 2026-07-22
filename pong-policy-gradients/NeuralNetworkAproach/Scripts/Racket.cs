using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Racket : MonoBehaviour
{
    Rigidbody2D rb;
    public float velocity;

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        rb.gravityScale = 0;
    }

    void Update()
    {
        rb.velocity = new Vector3(Input.GetAxis("Vertical") * velocity, 0);	
	}
}
