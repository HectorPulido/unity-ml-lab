using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Individuo
{
    public int Genoma;
    public float Puntaje;
    
    public string Sgenoma
    {
        get
        {
            return DecimalToBinario(Genoma);
        }
    }

    public string GetSgenoma
    {
        set
        {
            Genoma = BinaryToDecimal(int.Parse(value));

        }
    }

    public static string DecimalToBinario(int num)
    {
        int n = Mathf.Abs(num);// error

        string bin = "";

        if (n == 0)
            return "00000000000";
        while (n > 0)
        {
            if (n % 2 == 0)
                bin = "0" + bin;
            else
                bin = "1" + bin;
            n = (int)n / 2;
        }

        while (bin.Length < 10) //FORZAR 10 bits
        {
            bin = "0" + bin;
        }
        if (num > 0)
            return "0" + bin;
        else
            return "-" + bin;
    }

    public static int BinaryToDecimal(int n)
    {
        if (n == 0)
            return 0;
        float sum = 0;
        int strn = n.ToString().Length;
        for (int i = 0; i < strn; i++)
        {
            int lastDigit = n % 10;
            sum = sum + lastDigit * (Mathf.Pow(2, i));
            n = n / 10;
        }
        if (n > 0)
            return (int)sum;
        else
            return -(int)sum;
    }
}


public class AlgoritmoGenetico1 : MonoBehaviour
{
    public int CantidadDeIndividuos = 20;
    public int CantidadDeIteraciones = 50;

    public List<Individuo> poblacion;

    void Start()
    {
        CrearPoblacion();
        for (int i = 0; i < CantidadDeIteraciones; i++)
        {
            DeterminarPuntajes();
            Organizar();

            GameObject go = new GameObject(i.ToString());
            go.transform.position = new Vector3(poblacion[0].Genoma / 100f, MiFuncion(poblacion[0].Genoma / 100f));
            Mostrar();

            Combinacion();
        }
        print("FIN");
    }

    void CrearPoblacion()
    {
        poblacion = new List<Individuo>();
        for (int i = 0; i < CantidadDeIndividuos; i++)
        {
            Individuo ind = new Individuo();
            ind.Genoma = Random.Range(-25, 25);
            poblacion.Add(ind);
        }
    }
    void DeterminarPuntajes()
    {
        for (int i = 0; i < poblacion.Count; i++)
        {
            Individuo ind = poblacion[i];
            ind.Puntaje = 1 / MiFuncion(poblacion[i].Genoma / 100f);
            poblacion[i] = ind;
        }

    }
    void Organizar()
    {
        //Metodo de burbuja
        bool sw = false;
        while (!sw)
        {
            sw = true;
            for (int i = 1; i < poblacion.Count; i++)
            {
                if (poblacion[i].Puntaje > poblacion[i - 1].Puntaje)
                {
                    Individuo ind = poblacion[i];
                    poblacion[i] = poblacion[i - 1];
                    poblacion[i - 1] = ind;
                    sw = false;
                }
            }
        }
    }
    void Mostrar()
    {
        string s = "";

        for (int i = 0; i < poblacion.Count; i++)
        {
            s = s + " " + "(" + i + ")"  + poblacion[i].Genoma / 100f + " p:" + poblacion[i].Puntaje;
        }
        print(s);
    }

    [Range(0, 100)]
    public int RatioDeMutacion = 10;
    [Range(0, 100)]
    public int RatioDeMantenimiento = 50;

    void Combinacion()
    {
        List<Individuo> Supervivientes = BorrarInservibles();
        int count = Supervivientes.Count;//Dile no a la pedofilia!
        //Agregar Hijos
        for (int i = 0; i < (CantidadDeIndividuos * (1f - RatioDeMantenimiento / 100f)); i++)
        {
            string Sgen1 = Supervivientes[Random.Range(0, count)].Sgenoma;
            string Sgen2 = Supervivientes[Random.Range(0, count)].Sgenoma;

            string SgenHijo = "";

            //"-000000100100"


            for (int j = 0; j < Sgen1.Length; j++)
            {
                if (Random.Range(0, 100) <= RatioDeMutacion)
                {
                    if(j == 0)
                        SgenHijo += (Random.Range(0, 2) == 0) ? "0" : "-";
                    else
                        SgenHijo += Random.Range(0,2); // Mutacion
                }
                else
                {
                    //Combinacion
                    if (Random.Range(0, 100) > 50) 
                        SgenHijo += Sgen1[j];
                    else
                        SgenHijo += Sgen2[j];
                }
            }
            Individuo ind = new Individuo();
            ind.GetSgenoma = SgenHijo;
            Supervivientes.Add(ind);
        }
        poblacion = new List<Individuo>();
        poblacion = Supervivientes;
    }
    List<Individuo> BorrarInservibles()
    {
        List<Individuo> Supervivientes = new List<Individuo>();
        for (int i = 0; i < CantidadDeIndividuos * RatioDeMantenimiento / 100f; i++)
        {
            Supervivientes.Add(poblacion[i]);
        }
        return Supervivientes;
    }

    float MiFuncion(float x)
    {
        return (9 * (x + 8)) / (2 * (x + 8) + Mathf.Sin(x + 8));
    }
}
