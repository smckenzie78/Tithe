using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    PlayerState playerState;
    InventoryManager inventoryManager;
    GridManager gridManager;
    

    void Awake()
    {
        playerState = gameObject.GetComponent<PlayerState>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        gridManager = FindAnyObjectByType<GridManager>();
    }
    void TryInteract()
    {
        RaycastHit hit;
        Vector2Int cell;
        Vector3 combinedDirection = (transform.forward + Vector3.down).normalized;

        Physics.Raycast(transform.position, combinedDirection, out hit, 1);
        cell = gridManager.WorldToCell(hit.point);
        Debug.Log(cell.x.ToString() + cell.y.ToString());
        Debug.Log(gridManager.IsOccupied(cell));
        if(gridManager.IsOccupied(cell) && gridManager.GetTerrainType(cell) == 1)
        {
            gridManager.TryPlantingCrop(cell);
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
