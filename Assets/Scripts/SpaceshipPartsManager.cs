using System;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipPartsManager : MonoBehaviour
{
    [SerializeField] private int partsRequired = 3;
    [SerializeField] private string partsTag;

    public bool debug = true;
    
    public List<GameObject> partsCollected = new ();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(partsTag))
        {
            CollectItem(other.gameObject);
        }
    }

    private void CollectItem(GameObject part)
    {
        if (partsCollected.Contains(part))
            return;
            
        partsCollected.Add(part);
        
        part.GetComponent<ShipPart>().LockPickup();
        
        if(debug)
            Debug.Log("Item collected");

        if (partsCollected.Count >= partsRequired)
        {
            if(debug)
                Debug.Log("WIN!");
        }
    }
}
