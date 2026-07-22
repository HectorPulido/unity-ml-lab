using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Individuo3
{
    public float AmplitudL;
    public float AmplitudR;
    public float OffsetXR;
    public float OffsetXL;
    public float OffsetYR;
    public float OffsetYL;
    public float FactorL;
    public float FactorR;

    public float Puntaje;
}

public class Movimiento : MonoBehaviour
{
    Rigidbody R;
    Rigidbody L;
    // Use this for initialization

    [Range(-20 , 20)]
    public float AmplitudL;
    [Range(-20, 20)]
    public float AmplitudR;
    [Range(0, 1)]
    public float OffsetXR;
    [Range(0,1)]
    public float OffsetXL;
    [Range(-100, 100)]
    public float OffsetYR;
    [Range(-100, 100)]
    public float OffsetYL;
    [Range(-10, 10)]
    public float FactorL;
    [Range(-10, 10)]
    public float FactorR;

    public void SetUp(Individuo3 ind)
    {
        AmplitudL = ind.AmplitudL;
        AmplitudR = ind.AmplitudR;
        OffsetXR = ind.OffsetXR;
        OffsetXL = ind.OffsetXL;
        OffsetYR = ind.OffsetYR;
        OffsetYL = ind.OffsetYL;
        FactorL = ind.FactorL;
        FactorR = ind.FactorR;
    }

    void Start ()
    {
        R = transform.FindChild("R").GetComponent<Rigidbody>();
        L = transform.FindChild("L").GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 RZ = R.rotation.eulerAngles;
        Vector3 LZ = L.rotation.eulerAngles;
        RZ.z = OffsetYR + AmplitudR*Mathf.Sin(Time.time * FactorR + OffsetXR*Mathf.PI);
        LZ.z = OffsetYL + AmplitudL*Mathf.Sin(Time.time * FactorL + OffsetXL*Mathf.PI);
        R.rotation = Quaternion.Euler(RZ);
        L.rotation = Quaternion.Euler(LZ);

    }
}
