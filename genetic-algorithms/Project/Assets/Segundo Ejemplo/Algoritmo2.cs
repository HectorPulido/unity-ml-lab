using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Individuo2
{
    public Vector3[] Nodos;
    public float Puntaje;
}


public class Algoritmo2 : MonoBehaviour
{
    public Transform[] Nodos;

    public List<Individuo2> poblacion = new List<Individuo2>();

    public int Iteraciones = 50;
    IEnumerator Start()
    {
        lr = GetComponent<LineRenderer>();
        Iniciar();
        for (int i = 0; i < Iteraciones; i++)
        {
            Probar();
            Organizar();
            yield return new WaitForSeconds(0.2f);
            Mostrar();
            Combinar();
        }
        print("FIN");
        print("Distancia Minima: " + 1f/poblacion[0].Puntaje);
    }

    LineRenderer lr;
    void Mostrar()
    {
        lr.positionCount = Nodos.Length + 1;
        lr.SetPositions(poblacion[0].Nodos);
        lr.SetPosition(Nodos.Length, poblacion[0].Nodos[0]);               
    }

    public int CantidadDeIndividuos = 10;
    void Iniciar()
    {
        for (int i = 0; i < CantidadDeIndividuos; i++)
        {
            Individuo2 ind = new Individuo2();

            List<Transform> usables = new List<Transform>();
            usables.AddRange(Nodos);
            usables = DesordenarLista<Transform>(usables);

            List<Vector3> NodosIndividuo = new List<Vector3>();

            for (int j = 0; j < usables.Count; j++)
            {
                NodosIndividuo.Add(usables[j].position);
            }

            ind.Nodos = NodosIndividuo.ToArray();
            poblacion.Add(ind);
        }
    }
    List<T> DesordenarLista<T>(List<T> input)
    {
        List<T> arr = input;
        List<T> arrDes = new List<T>();

        while (arr.Count > 0)
        {
            int val = Random.Range(0, arr.Count - 1);
            arrDes.Add(arr[val]);
            arr.RemoveAt(val);
        }

        return arrDes;
    }

    void Probar()
    {
        for (int i = 0; i < poblacion.Count; i++)
        {
            float distancia = 0;
            for (int k = 0; k < Nodos.Length - 1; k++)
            {
                distancia += Vector3.Distance(poblacion[i].Nodos[k], poblacion[i].Nodos[k + 1]);
            }
            Individuo2 ind = poblacion[i];
            ind.Puntaje = 1f / distancia;
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
                    Individuo2 ind = poblacion[i];
                    poblacion[i] = poblacion[i - 1];
                    poblacion[i - 1] = ind;
                    sw = false;
                }
            }
        }
    }

    [Range(0,100)]
    public int RatioDeMantenimiento = 40;
    [Range(0,100)]
    public int ProbabilidadDeMutacion = 20;
    void Combinar()
    {
        List<Individuo2> supervivientes = BorrarInservibles();
        int count = supervivientes.Count;
        int faltantes = poblacion.Count - count;

        for (int i = 0; i < faltantes; i++)
        {
            Individuo2 padre1 = supervivientes[Random.Range(0, count)];
            Individuo2 padre2 = supervivientes[Random.Range(0, count)];

            Individuo2 hijo = new Individuo2();
            hijo.Nodos = new Vector3[Nodos.Length];

            // 4231
            // 2413
            // 4231


            //Subruta
            int Inicio = Random.Range(0, Nodos.Length - 2);
            int final = Random.Range(Inicio, Nodos.Length);
            for (int j = Inicio; j < final; j++)
            {
                hijo.Nodos[j] = padre1.Nodos[j];
            }
            //Preparar padre2
            List<Vector3> v = new List<Vector3>();
            v.AddRange(padre2.Nodos);
            for (int j = Inicio; j < final; j++)
            {
                v.Remove(padre1.Nodos[j]);
            }

            //Llenar espacio
            int c = 0;
            for (int j = 0; j < Nodos.Length; j++)
            {
                if (hijo.Nodos[j] == Vector3.zero)
                {
                    hijo.Nodos[j] = v[c];
                    c++;
                }
            }          

            //Mutacion
            if (Random.Range(0, 100) <= ProbabilidadDeMutacion)
            {
                int g1 = Random.Range(0, Nodos.Length);
                int g2 = Random.Range(0, Nodos.Length);

                Vector2 aux = hijo.Nodos[g1];
                hijo.Nodos[g1] = hijo.Nodos[g2];
                hijo.Nodos[g2] = aux;
            }

            supervivientes.Add(hijo);
        }

        poblacion = supervivientes;
    }
    List<Individuo2> BorrarInservibles()
    {
        List<Individuo2> Supervivientes = new List<Individuo2>();
        for (int i = 0; i < CantidadDeIndividuos * RatioDeMantenimiento / 100f; i++)
        {
            Supervivientes.Add(poblacion[i]);
        }
        return Supervivientes;
    }


}
