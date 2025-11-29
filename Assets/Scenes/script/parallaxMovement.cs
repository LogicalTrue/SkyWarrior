using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxMovement : MonoBehaviour
{
    [Header("Configuración General")]
    public Transform followCam;
    [Range(0.01f, 1f)]
    public float parallaxSpeed = 0.5f;
    public float fadeDuration = 2.0f;

    [Header("Capas")]
    public List<Renderer> forestLayers;
    public List<Renderer> desertLayers;

    [Header("Clima")]
    public ParticleSystem hojasFX;
    public ParticleSystem polvoFX;

    // Variables internas
    private Transform cam;
    private Vector3 camStartPos;
    private float farthestBack;
    private Dictionary<Renderer, float> layerSpeeds = new Dictionary<Renderer, float>();
    
    private bool showingForest = true;
    private float transitionAlpha = 0.0f; 

    void Start()
    {
        cam = followCam;
        camStartPos = cam.position;

        CalculateFarthestBack();
        CalculateSpeeds(forestLayers);
        CalculateSpeeds(desertLayers);

        transitionAlpha = 0.0f; 
        UpdateMaterialsAlpha();

        // INICIO: Hojas prendidas, Polvo apagado (pero corriendo con emisión 0)
        if (hojasFX != null) hojasFX.Play();
        if (polvoFX != null)
        {
            polvoFX.Play();
            var emission = polvoFX.emission;
            emission.rateOverTime = 0; // Arranca sin tirar polvo
        }
    }

    // ... (CalculateFarthestBack, CalculateSpeeds y LateUpdate QUEDAN IGUAL) ...
    // ... (Solo copio lo que cambia para no llenarte de texto, mantené tus funciones de calculo) ...

    void CalculateFarthestBack()
    {
        // (Tu código de siempre aquí)
        farthestBack = 0;
        foreach (var r in forestLayers) { if ((r.transform.position.z - cam.position.z) > farthestBack) farthestBack = r.transform.position.z - cam.position.z; }
        foreach (var r in desertLayers) { if ((r.transform.position.z - cam.position.z) > farthestBack) farthestBack = r.transform.position.z - cam.position.z; }
    }

    void CalculateSpeeds(List<Renderer> layers)
    {
        // (Tu código de siempre aquí)
        foreach (var r in layers) {
            float dist = r.transform.position.z - cam.position.z;
            float speed = (farthestBack > 0) ? 1 - (dist / farthestBack) : 0;
            if(!layerSpeeds.ContainsKey(r)) layerSpeeds.Add(r, speed);
        }
    }

    void LateUpdate()
    {
        float dist = cam.position.x - camStartPos.x;
        ApplyParallax(forestLayers, dist);
        ApplyParallax(desertLayers, dist);

        float target = showingForest ? 0.0f : 1.0f;
        if (Mathf.Abs(transitionAlpha - target) > 0.01f)
        {
            transitionAlpha = Mathf.MoveTowards(transitionAlpha, target, Time.deltaTime / fadeDuration);
            UpdateMaterialsAlpha();
        }
    }

    void ApplyParallax(List<Renderer> layers, float distance)
    {
        foreach (var r in layers)
        {
            if (layerSpeeds.ContainsKey(r))
            {
                float speed = layerSpeeds[r] * parallaxSpeed;
                r.material.SetTextureOffset("_MainTex", new Vector2(distance, 0) * speed);
            }
        }
    }

    void UpdateMaterialsAlpha()
    {
        float forestAlpha = 1.0f - transitionAlpha;
        float desertAlpha = transitionAlpha;

        SetLayerAlpha(forestLayers, forestAlpha);
        SetLayerAlpha(desertLayers, desertAlpha);
    }

    void SetLayerAlpha(List<Renderer> layers, float alpha)
    {
        foreach (var r in layers)
        {
            Color c = r.material.color;
            c.a = alpha;
            r.material.color = c;
        }
    }

    // --- ACÁ ESTÁ LA MAGIA DE LA TRANSICIÓN SUAVE ---

    public void SwitchToDesert()
    {
        showingForest = false;

        // Hojas: Stop (Dejan de nacer, las viejas caen hasta morir)
        if (hojasFX != null) hojasFX.Stop(); 

        // Polvo: Subimos la emisión para que empiece a aparecer de a poco
        if (polvoFX != null)
        {
            var emission = polvoFX.emission;
            emission.rateOverTime = 50; // Cantidad de polvo deseada
        }
    }

    public void SwitchToForest()
    {
        showingForest = true;

        // Hojas: Play (Empiezan a caer de nuevo)
        if (hojasFX != null) hojasFX.Play();

        // Polvo: Bajamos la emisión a 0 (Dejan de nacer, las viejas se van yendo)
        if (polvoFX != null)
        {
            var emission = polvoFX.emission;
            emission.rateOverTime = 0;
        }
    }
}