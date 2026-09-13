using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    private Collider col;

    public Transform pairedPortalTeleportPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(pairedPortalTeleportPoint != null)
                other.transform.parent.transform.position = pairedPortalTeleportPoint.position;
        }
    }
}
