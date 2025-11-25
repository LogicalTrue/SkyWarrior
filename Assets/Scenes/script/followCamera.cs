using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCamera : MonoBehaviour
{
    public Transform targetPlayer;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);

    // 1. Agregamos el "interruptor". Por defecto, está encendido.
    private bool isFollowing = true;

    void Update()
    {
        // 2. Ahora la condición es doble: que haya un objetivo Y que el interruptor esté encendido.
        if (targetPlayer != null && isFollowing)
        {
            transform.position = targetPlayer.position + offset;
        }
    }

    // 3. ¡Este es el método público que pediste!
    // Lo llamaremos desde otro script para detener la cámara.
    public void StopFollowing()
    {
        isFollowing = false; // Apagamos el interruptor.
        Debug.Log("La cámara se ha detenido.");
    }
}