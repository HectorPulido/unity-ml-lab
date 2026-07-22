using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectMovementCartPole : MonoBehaviour
{
    public float force;
    public Transform pole;

    private Rigidbody2D poleRb;
    private Rigidbody2D rb;

    public bool Reward
    {
        get
        {
            var angle = Mathf.Abs(pole.eulerAngles.z % 360);
            return (angle > 350 || angle < 10) && Mathf.Abs(transform.position.x) < 4;
        }
    }
    public float AnglePole
    {
        get
        {
            return pole.eulerAngles.z % 360 / 360f;
        }
    }

    public float CartPosition
    {
        get
        {
            return Mathf.Abs(transform.position.x) / 7f;
        }
    }
    public float CartSpeed
    {
        get
        {
            return rb.velocity.x / 10;
        }
    }

    public float PoleAngularSpeed
    {
        get
        {
            return poleRb.angularVelocity;
        }
    }

    private Vector3 initialPositionCart;
    private Vector3 initialPositionPole;


    void Start()
    {
        initialPositionCart = transform.position;
        initialPositionPole = pole.position;
        rb = GetComponent<Rigidbody2D>();
        poleRb = pole.GetComponent<Rigidbody2D>();
    }

    [HideInInspector]
    public float input;

    void Update()
    {
        //input = Input.GetAxis("Horizontal");
        if (0.5f > Mathf.Abs(input))
        {
            return;
        }

        if (input > 0)
        {
            rb.AddForce(new Vector2(force, 0));
        }
        else
        {
            rb.AddForce(new Vector2(force * -1, 0));
        }
    }


    public void ResetEnv()
    {
        transform.position = initialPositionCart;
        pole.position = initialPositionPole;
        pole.eulerAngles = new Vector3(0,0, Random.Range(-2f, 2));
        rb.velocity = Vector2.zero;
        poleRb.angularVelocity = 0;
    }

}
