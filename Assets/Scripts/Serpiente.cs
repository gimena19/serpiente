using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Serpiente : MonoBehaviour
{
    public GameObject Bloque;
    public GameObject Item;//prefap

    public GameObject Escenario;
    public int Ancho, Alto;

    private Queue<GameObject> cuerpo = new Queue<GameObject>();
    private GameObject cabeza;
    private GameObject item; //escena
    private Vector3 direccion = Vector3.right;

    private enum TipoCasilla
    {
        Vacio,Obstaculo,Item
    }

    private TipoCasilla[,] mapa;

    private void Awake()
    {
        mapa = new TipoCasilla[Ancho, Alto];
        CrearMuros();
        int posicionInicialX = Ancho / 2;
        int posicionInicialY = Alto / 2;
        
        cabeza = NuevoBloque(posicionInicialX, posicionInicialY);
        InstanciarItemEnPosicionAleatoria();
        StartCoroutine(Movimiento());
        
    }

    private void MoverItemPosicionAleatoria()
    {
        Vector2Int posicion = ObtenerPosicionVacia();
        item.transform.position = new Vector3(posicion.x, posicion.y);
        EstablecerMapa(item.transform.position, TipoCasilla.Item);
    }

    private void InstanciarItemEnPosicionAleatoria()
    {
        Vector2Int posicion = ObtenerPosicionVacia();/////////////////////////////////
        item = NuevoItem(posicion.x, posicion.y);
    }

    private Vector2Int ObtenerPosicionVacia()
    {
        List<Vector2Int> posicionesVacias = new List<Vector2Int>();
        for(int x=0; x < Ancho; x++)
        {
            for(int y = 0; y < Alto; y++)
            {
                if(mapa[x,y] == TipoCasilla.Vacio)
                {
                    posicionesVacias.Add(new Vector2Int(x, y));
                }
            }
        }
        return posicionesVacias[Random.Range(0,posicionesVacias.Count)];
    }

    private TipoCasilla ObtenerMapa (Vector3 posicion)
    {
        return mapa[Mathf.RoundToInt(posicion.x), Mathf.RoundToInt(posicion.y)];

    }

    private void EstablecerMapa(Vector3 posicion, TipoCasilla valor)
    {
        mapa[Mathf.RoundToInt(posicion.x), Mathf.RoundToInt(posicion.y)] = valor;
    }


    private IEnumerator Movimiento()
    {
        WaitForSeconds espera = new WaitForSeconds(0.15f);
        while (true)
        {
            Vector3 nuevaPosicion = cabeza.transform.position + direccion;
            TipoCasilla casillaAOcupar = ObtenerMapa(nuevaPosicion);
            if(casillaAOcupar == TipoCasilla.Obstaculo)
            {
                Debug.Log("Muerto!");
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                yield break;
            }
            else
            {
                GameObject parteCuerpo;
                if(casillaAOcupar == TipoCasilla.Item)
                {
                    parteCuerpo = NuevoBloque(nuevaPosicion.x, nuevaPosicion.y);
                    MoverItemPosicionAleatoria();
                }
                parteCuerpo = cuerpo.Dequeue();
                EstablecerMapa(parteCuerpo.transform.position, TipoCasilla.Vacio);
                parteCuerpo.transform.position = nuevaPosicion;
                EstablecerMapa(nuevaPosicion, TipoCasilla.Obstaculo);
                cuerpo.Enqueue(parteCuerpo);

                cabeza = parteCuerpo;
                yield return espera;

            }


        }
    }

    private GameObject NuevoBloque(float x, float y)
    {
        GameObject nuevo = Instantiate(Bloque, new Vector3(x, y), Quaternion.identity, this.transform);
        cuerpo.Enqueue(nuevo);
        EstablecerMapa(nuevo.transform.position, TipoCasilla.Obstaculo);
        return nuevo;
    }

    private GameObject NuevoItem(int x, int y)
    {
        GameObject nuevo = Instantiate(Item, new Vector3(x, y), Quaternion.identity, Escenario.transform);
        EstablecerMapa(nuevo.transform.position, TipoCasilla.Item);
        return nuevo;
    }




    private void CrearMuros()
    {
        for(int x = 0; x < Ancho; x++)
        {
            for(int y = 0; y < Alto; y++)
            {
                if (x== 0|| x==Ancho-1 || y==0 ||y==Alto -1)
                {
                    Vector3 posicion = new Vector3(x, y);
                    Instantiate(Bloque, posicion, Quaternion.identity, Escenario.transform);
                    EstablecerMapa(posicion, TipoCasilla.Obstaculo);
                }

            }
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direccionSeleccionada = new Vector3(horizontal, vertical);
        if(direccionSeleccionada != Vector3.zero)
        {
            direccion = direccionSeleccionada;
        }
    }

}
