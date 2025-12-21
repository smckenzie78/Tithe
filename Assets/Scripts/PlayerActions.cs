using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    PlayerState playerState;
    InventoryManager inventoryManager;

    void Awake()
    {
        playerState = gameObject.GetComponent<PlayerState>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }
    void TryInteract()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, fwd, out hit, 1))
        {
            if (hit.collider.gameObject.tag == "Soil")
            {
                hit.collider.gameObject.GetComponent<SoilTile>().soilInteraction();
            }
            if (hit.collider.gameObject.tag == "NPC")
            {
                hit.collider.gameObject.GetComponent<NPCBehavior>().NPCInteraction();
            }
        }
    }


    void Update()
    {
        if (playerState.Busy) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryManager.showInventory();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryInteract();
        }
    }
}
