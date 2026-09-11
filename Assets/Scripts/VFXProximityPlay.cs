using System;
using UnityEngine;
using UnityEngine.VFX;

public class VFXProximityPlay : MonoBehaviour
{
    private VisualEffect effect;

    private void Start()
    {
        effect = GetComponent<VisualEffect>();
        effect.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            effect.Play();
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            effect.Stop();
    }
}
