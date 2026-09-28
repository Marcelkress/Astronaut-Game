using System;
using System.Collections;
using UnityEngine;

public class ShipPart : MonoBehaviour
{
    private Canvas UICanvas;
    private bool displayUI;
    private Rigidbody rb;
    private bool isHeld;
    public LayerMask playerLayer;

    public float UIOffset = 2;

    public bool canPickup { get; private set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canPickup = true;
        rb = GetComponent<Rigidbody>();
        UICanvas = GetComponentInChildren<Canvas>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup)
            return;
        
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
        if (!canPickup)
            return;
            
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

    public void LockPickup()
    {
        StartCoroutine(WaitForDrop());
    }

    private IEnumerator WaitForDrop()
    {
        yield return new WaitUntil(() => !isHeld);
        canPickup = false;
        
        //Debug.Log("Locked pickup");
    }
}
