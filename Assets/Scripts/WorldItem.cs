using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    private InventoryManager inventoryManager;
    SphereCollider sphereCollider;
    GameObject player;
    private bool collected = false;
    void Awake()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        player = GameObject.Find("Player");
        sphereCollider = gameObject.GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (collected)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 10);
            if (transform.position == player.transform.position)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        sphereCollider.enabled = false;
        collected = true;
        inventoryManager.addItem(itemData, 1);
    }
}
