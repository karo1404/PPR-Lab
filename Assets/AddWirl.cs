using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWirl : MonoBehaviour
{
    private Material material;
    public float intensity;
    public float phase = 1;
    public float freq = 1;
    public float amp = 1;
    void Awake()
    {
        material = new Material(Shader.Find("Hidden/Wirl"));

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        float[] xs = new float[] { 0.2f, 0.7f };
        float[] ys = new float[] { 0.1f, 0.75f };
        material.SetFloatArray("_xc", xs);
        material.SetFloatArray("_yc", ys);
        material.SetFloat("_count", 2);
        material.SetFloat("_phase", phase);
        material.SetFloat("_freq", freq);
        material.SetFloat("_amp", amp);
        material.SetFloat("_intensity", intensity);
        material.SetFloat("_seed", Random.value);
        Graphics.Blit(source, destination, material);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
