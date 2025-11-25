using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuSystem : MonoBehaviour
{
    // Hacemos el método PÚBLICO para que el botón lo vea en el Inspector.
    public void Jugar()
    {
        // Corregimos el nombre del método a "GetActiveScene".
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Este también tiene que ser PÚBLICO.
    public void Salir()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para que veas que funciona en el Editor.
        Application.Quit();
    }
}