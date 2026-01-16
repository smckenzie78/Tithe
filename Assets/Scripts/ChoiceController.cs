using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    PlayerState playerState;
    public GameObject buttonSelect;
    public TextMeshProUGUI textComponent;
    private bool active = false;
    private System.Action<ItemData> onSelected;
    private List<ItemData> items;
    void Awake()
    {
        playerState = FindAnyObjectByType<PlayerState>();
    }

    void Start()
    {
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
    }

    public void YesOrNo()
    {
        
    }

    public void StartItemSelection(Dictionary<ItemData, int> items, System.Action<ItemData> callback)
    {
        onSelected = callback;

        active = true;
        playerState.Busy = true;
        canvas.enabled = true;

        var posY = 50;

        foreach (ItemData item in items.Keys)
        {
            GameObject newButton = Instantiate(buttonSelect, this.gameObject.transform);
            newButton.GetComponent<Button2>().itemData = item;
            newButton.GetComponent<Button2>().text = newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            newButton.GetComponent<Button2>().text.text = item.itemName;
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(0, posY, 0);
            posY += 50;

            newButton.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(item); Destroy(newButton); });
        }
    }

    private void SelectItem(ItemData item)
    {
        active = false;
        playerState.Busy = false;
        canvas.enabled = false;
        onSelected?.Invoke(item);
    }

    public bool IsActive()
    {
        return active;
    }
}
