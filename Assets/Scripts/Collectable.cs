using System;
using UnityEngine;

public class Collectable : MonoBehaviour, ICollectable
{
    private Canvas UICanvas;
    private bool displayUI;
    private Rigidbody rb;
    private bool isHeld;
    public LayerMask playerLayer;

    public float UIOffset = 2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UICanvas = GetComponentInChildren<Canvas>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            displayUI = true;
            UICanvas.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            displayUI = false;
            UICanvas.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (displayUI && isHeld == false) 
        {
            UICanvas.transform.position =
                new Vector3(transform.position.x, transform.position.y + UIOffset, transform.position.z);
            
            UICanvas.transform.LookAt(Camera.main.transform);
        }
        else
        {
            UICanvas.enabled = false;
        }
    }

    public void Pickup()
    {
        rb.excludeLayers += playerLayer;
        isHeld = true;
        rb.isKinematic = true;
    }

    public void Drop()
    {
        rb.excludeLayers = 0;
        isHeld = false;
        rb.isKinematic = false;
    }
}
