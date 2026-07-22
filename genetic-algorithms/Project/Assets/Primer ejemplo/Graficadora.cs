using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graficadora : MonoBehaviour
{

    /// <summary>
    ///  f(x) = (9(x+8))/(2(x+8)+sin(x+8)) 
    /// </summary>

    LineRenderer ren;
    public Vector2 Rango = new Vector2(-10,10);
    public float Precision = 0.01f;

	void Start () {
        ren = GetComponent<LineRenderer>();
        Graficar();
	}

    void Graficar()
    {
        ren.positionCount = (int)(Mathf.Abs(Rango.x / Precision) + Mathf.Abs(Rango.y / Precision));
        ren.SetPositions(ListaDeValores().ToArray());
    }

    List<Vector3> ListaDeValores()
    {
        List<Vector3> f = new List<Vector3>() ;
        for (int i = (int)(Rango.x / Precision); i < (int)(Rango.y / Precision); i++)
        {
            f.Add(new Vector3(i * Precision, MiFuncion((float)i * Precision)));
        }
        return f;
    }

    float MiFuncion(float x)
    {
        return (9 * (x + 8)) / (2 * (x + 8) + Mathf.Sin(x + 8));
    }
}
