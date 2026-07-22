using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgoritmoGenetico3 : MonoBehaviour
{
    IEnumerator Start()
    {
        PoblacionInicial();
        for (int i = 0; i < NumeroDeIteraciones; i++)
        {
            print("ROUND: " + i);
            Instanciar();
            yield return new WaitForSeconds(60);
            Puntuar();
            Organizar();
            Combinar();
        }
        GameObject go = Instantiate(Prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        go.GetComponent<Movimiento>().SetUp(poblacion[0]);
        individuos.Add(go);
        print("Fin");
    }
    public GameObject Prefab;
    public int CantidadDeIndividuos = 15;
    public int NumeroDeIteraciones = 10;

    List<Individuo3> poblacion = new List<Individuo3>();
    List<GameObject> individuos = new List<GameObject>();

    void PoblacionInicial()
    {
        for (int i = 0; i < CantidadDeIndividuos; i++)
        {
            Individuo3 ind = new Individuo3();
            ind.AmplitudL = Random.Range(-20, 20);
            ind.AmplitudR = Random.Range(-20, 20);
            ind.OffsetXL = Random.Range(0, 1);
            ind.OffsetXR = Random.Range(0, 1);
            ind.OffsetYL = Random.Range(-30, 30);
            ind.OffsetYR = Random.Range(-30, 30);
            ind.FactorL = Random.Range(-10, 10);
            ind.FactorR = Random.Range(-10, 10);
            poblacion.Add(ind);
        }
    }
    void Instanciar()
    {
        for (int i = 0; i < CantidadDeIndividuos; i++)
        {
            GameObject go = Instantiate(Prefab, new Vector3(0, 0, i*2), Quaternion.identity) as GameObject;
            go.GetComponent<Movimiento>().SetUp(poblacion[i]);
            individuos.Add(go);
        }
    }
    void Puntuar()
    {
        for (int i = 0; i < CantidadDeIndividuos; i++)
        {
            float puntuacion = individuos[i].transform.position.x;
            puntuacion = puntuacion * ((individuos[i].transform.position.y > 10 || individuos[i].transform.position.y < -5) ? 0.1f : 1);
            Individuo3 ind = poblacion[i];
            ind.Puntaje = puntuacion;
            poblacion[i] = ind;
            Destroy(individuos[i]);
        }
        individuos = new List<GameObject>();
    }
    void Organizar()
    {
        //Metodo de burbuja
        bool sw = false; // WRONG
        while (!sw)
        {
            sw = true;
            for (int i = 1; i < poblacion.Count; i++)
            {
                if (poblacion[i].Puntaje > poblacion[i - 1].Puntaje)
                {
                    Individuo3 ind = poblacion[i];
                    poblacion[i] = poblacion[i - 1];
                    poblacion[i - 1] = ind;
                    sw = false;
                }
            }
        }
    }
    public int RatioMantenimiento = 30;
    public int RatioMutacion = 20;
    void Combinar()
    {
        List<Individuo3> Supervivientes = BorrarInservibles();
        int count = Supervivientes.Count;//Dile no a la pedofilia!
        int faltantes = poblacion.Count - count;
        for (int i = 0; i < faltantes; i++)
        {
            Individuo3 padre1 = Supervivientes[Random.Range(0, count)];
            Individuo3 padre2 = Supervivientes[Random.Range(0, count)];

            Individuo3 ind = new Individuo3();
            ind.AmplitudL = (Random.Range(0,100) > 50) ? padre1.AmplitudL : padre2.AmplitudL;
            ind.AmplitudR = (Random.Range(0, 100) > 50) ? padre1.AmplitudR : padre2.AmplitudR;
            ind.FactorL = (Random.Range(0, 100) > 50) ? padre1.FactorL : padre2.FactorL;
            ind.FactorR = (Random.Range(0, 100) > 50) ? padre1.FactorR : padre2.FactorR;
            ind.OffsetXL = (Random.Range(0, 100) > 50) ? padre1.OffsetXL : padre2.OffsetXL;
            ind.OffsetXR = (Random.Range(0, 100) > 50) ? padre1.OffsetXR : padre2.OffsetXR;
            ind.OffsetYL = (Random.Range(0, 100) > 50) ? padre1.OffsetYL : padre2.OffsetYL;
            ind.OffsetYR = (Random.Range(0, 100) > 50) ? padre1.OffsetYR : padre2.OffsetYR;

            ind.AmplitudL = (Random.Range(0, 100) > RatioMutacion) ? ind.AmplitudL : Random.Range(-20, 20);
            ind.AmplitudR = (Random.Range(0, 100) > RatioMutacion) ? ind.AmplitudR : Random.Range(-20, 20);
            ind.OffsetXL = (Random.Range(0, 100) > RatioMutacion) ? ind.OffsetXL : Random.Range(0, 1);
            ind.OffsetXR = (Random.Range(0, 100) > RatioMutacion) ? ind.OffsetXR : Random.Range(0, 1);
            ind.OffsetYL = (Random.Range(0, 100) > RatioMutacion) ? ind.OffsetYL : Random.Range(-30, 30);
            ind.OffsetYR = (Random.Range(0, 100) > RatioMutacion) ? ind.OffsetYR : Random.Range(-30, 30);
            ind.FactorL = (Random.Range(0, 100) > RatioMutacion) ? ind.FactorL : Random.Range(-10, 10);
            ind.FactorR = (Random.Range(0, 100) > RatioMutacion) ? ind.FactorR : Random.Range(-10, 10);

            Supervivientes.Add(ind);
        }
        poblacion = Supervivientes;
    }
    List<Individuo3> BorrarInservibles()
    {
        List<Individuo3> Supervivientes = new List<Individuo3>();
        for (int i = 0; i < CantidadDeIndividuos * RatioMantenimiento / 100f; i++)
        {
            Supervivientes.Add(poblacion[i]);
        }
        return Supervivientes;
    }
}
