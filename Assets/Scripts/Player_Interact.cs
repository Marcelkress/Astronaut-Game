using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interact : MonoBehaviour
{
    public bool debug;
    
    [Header("Sphere casting")]
    public LayerMask interactableLayer;
    public float spherecastRadius = 2;

    [Header("Holding item")] 
    public Transform holdPosition;
    public float lerpTime;

    private GameObject holdingItem;
    private ShipPart heldShipPart;
    
    private PlayerInput playerInput;
    private InputAction interactAction;
    private Vector3 smoothDampVelocity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        interactAction = playerInput.actions["Interact"];
        interactAction.performed += PickupItem;
        interactAction.canceled += DropItem;

    }

    void FixedUpdate()
    {
        if (holdingItem != null && heldShipPart.canPickup)
        {
            holdingItem.transform.position =
                Vector3.Lerp(holdingItem.transform.position, holdPosition.position, lerpTime);
        }
    }

    void PickupItem(InputAction.CallbackContext context)
    {
        if (debug)
            Debug.Log("PICKUP");

        var hits = Physics.SphereCastAll
            (transform.position, spherecastRadius, Vector3.up, spherecastRadius, interactableLayer);

        if (hits.Length != 0)
        {
            foreach (var hit in hits)
            {
                if (holdingItem == null)
                { 
                    holdingItem = hit.transform.gameObject;   
                }
                    
                // Pickup the closest item of all items within the pickup radius
                float distanceToCurrent = Vector3.Distance(holdingItem.transform.position, transform.position);
                float distanceToNext = Vector3.Distance(hit.transform.position, transform.position);
                if (distanceToNext < distanceToCurrent)
                { 
                    holdingItem = hit.transform.gameObject;
                }
            }
            
            // Tell the object we are picking it up
            if (holdingItem.transform.TryGetComponent(out ShipPart shipPart))
            {
                shipPart.Pickup();
                heldShipPart = shipPart;
            }
        }
    }

    void DropItem(InputAction.CallbackContext context)
    {
        if (debug)
            Debug.Log("DROP :((");

        if (holdingItem == null)
            return;
        
         holdingItem.GetComponent<ShipPart>().Drop();
         holdingItem = null;
    }
}


