using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxMovement : MonoBehaviour
{
    [Header("Configuración General")]
    public Transform followCam; // Tu cámara
    [Range(0.01f, 1f)]
    public float parallaxSpeed = 0.5f; // Velocidad general
    public float fadeDuration = 2.0f; // Cuánto tarda en desaparecer/aparecer

    [Header("Capas (Arrastralas acá manualmente)")]
    // Usamos listas para saber qué sprite es de qué bioma
    public List<Renderer> forestLayers;
    public List<Renderer> desertLayers;

    // Estado interno
    private Transform cam;
    private Vector3 camStartPos;
    private float farthestBack;
    
    // Diccionario para guardar la velocidad calculada de CADA renderer individualmente
    private Dictionary<Renderer, float> layerSpeeds = new Dictionary<Renderer, float>();

    // Control de transición (true = bosque, false = desierto)
    private bool showingForest = true;
    private float transitionAlpha = 0.0f; // 0 = Todo Bosque, 1 = Todo Desierto

    void Start()
    {
        cam = followCam;
        camStartPos = cam.position;

        // 1. Calculamos cuál es la capa más lejana de TODAS (bosque y desierto combinados)
        CalculateFarthestBack();

        // 2. Calculamos velocidades para TODAS las capas en base a esa distancia
        CalculateSpeeds(forestLayers);
        CalculateSpeeds(desertLayers);

        // 3. Inicializamos visibilidad (arrancamos mostrando el bosque)
        transitionAlpha = 0.0f; 
        UpdateAlpha();
    }

    void CalculateFarthestBack()
    {
        farthestBack = 0;
        // Revisamos bosque
        foreach (var r in forestLayers)
        {
            float dist = r.transform.position.z - cam.position.z;
            if (dist > farthestBack) farthestBack = dist;
        }
        // Revisamos desierto
        foreach (var r in desertLayers)
        {
            float dist = r.transform.position.z - cam.position.z;
            if (dist > farthestBack) farthestBack = dist;
        }
    }

    void CalculateSpeeds(List<Renderer> layers)
    {
        foreach (var r in layers)
        {
            float dist = r.transform.position.z - cam.position.z;
            // Evitamos división por cero si farthestBack es 0
            float speed = (farthestBack > 0) ? 1 - (dist / farthestBack) : 0;
            
            // Guardamos la velocidad asociada a este renderer especifico en el diccionario
            if(!layerSpeeds.ContainsKey(r))
            {
                layerSpeeds.Add(r, speed);
            }
        }
    }

    void LateUpdate()
    {
        // --- 1. Movimiento (Mueve TODO, aunque sea invisible) ---
        // Esto asegura que cuando aparezca el desierto, ya esté en la posición correcta
        float dist = cam.position.x - camStartPos.x;
        
        ApplyParallax(forestLayers, dist);
        ApplyParallax(desertLayers, dist);

        // --- 2. Transición de Alpha (Fade in / Fade out) ---
        float target = showingForest ? 0.0f : 1.0f;
        
        // Si no hemos llegado al objetivo de transparencia, interpolamos
        if (Mathf.Abs(transitionAlpha - target) > 0.01f)
        {
            transitionAlpha = Mathf.MoveTowards(transitionAlpha, target, Time.deltaTime / fadeDuration);
            UpdateAlpha();
        }
    }

    void ApplyParallax(List<Renderer> layers, float distance)
    {
        foreach (var r in layers)
        {
            if (layerSpeeds.ContainsKey(r))
            {
                float speed = layerSpeeds[r] * parallaxSpeed;
                // Movemos la textura (offset) igual que en tu script original
                r.material.SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
            }
        }
    }

    void UpdateAlpha()
    {
        // Forest se ve cuando transitionAlpha es 0
        float forestAlpha = 1.0f - transitionAlpha;
        // Desert se ve cuando transitionAlpha es 1
        float desertAlpha = transitionAlpha;

        SetLayerAlpha(forestLayers, forestAlpha);
        SetLayerAlpha(desertLayers, desertAlpha);
    }

    void SetLayerAlpha(List<Renderer> layers, float alpha)
    {
        foreach (var r in layers)
        {
            // OJO: Tu material debe soportar transparencia (Rendering Mode: Fade o Transparent)
            Color c = r.material.color;
            c.a = alpha;
            r.material.color = c;
        }
    }

    // --- MÉTODOS PÚBLICOS PARA LLAMAR DESDE EL TRIGGER ---
    
    // Llamar a este cuando tocas el floor_4
    public void SwitchToDesert()
    {
        showingForest = false;
    }

    // Llamar a este si volvés para atrás
    public void SwitchToForest()
    {
        showingForest = true;
    }
}