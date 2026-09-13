using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider))]
public class VFXProximityPlay : MonoBehaviour
{
    private VisualEffect effect;
    private Animator anim;
    
    private Coroutine fadeRoutine;
    private float currentEmission;
    
    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
        effect = GetComponent<VisualEffect>();
        effect.Stop();
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            effect.Play();
            anim.Play("FadeIn");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            effect.Stop();
            anim.Play("FadeOut");
        }
    }
    
}
