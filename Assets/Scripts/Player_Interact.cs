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

    private GameObject holdingItem;
    
    private PlayerInput playerInput;
    private InputAction interactAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        interactAction = playerInput.actions["Interact"];
        interactAction.performed += PickupItem;
        interactAction.canceled += DropItem;

    }

    void LateUpdate()
    {
        if(holdingItem != null)
            holdingItem.transform.position = holdPosition.position;
    }

    void PickupItem(InputAction.CallbackContext context)
    {
        if (debug)
            Debug.Log("PICKUP");

        var hits = Physics.SphereCastAll(transform.position, spherecastRadius, Vector3.up, spherecastRadius, interactableLayer);

        if (hits.Length != 0)
        {
            if (hits[0].transform.TryGetComponent<ICollectable>(out ICollectable collectable))
            {
                collectable.Pickup();   
                
                holdingItem = hits[0].transform.gameObject;
            }
        }
    }

    void DropItem(InputAction.CallbackContext context)
    {
        if (debug)
            Debug.Log("DROP :((");

        if (holdingItem == null)
            return;
        
         holdingItem.GetComponent<ICollectable>().Drop();
         holdingItem = null;
    }
}

public interface ICollectable
{
    public void Pickup();
    public void Drop();
}
