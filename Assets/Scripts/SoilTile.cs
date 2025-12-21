using System.Collections.Generic;
using UnityEngine;

public class SoilTile : MonoBehaviour
{
    public ItemData crop;
    InventoryManager inventoryManager;
    TimeManager timeManager;
    StateManager stateManager;
    DialogueController dialogue;
    ChoiceController choiceController;
    public bool containsCrop;
    public int daysSincePlanted;
    private float currentDay;

    void Awake()
    {
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        timeManager = FindAnyObjectByType<TimeManager>();
        stateManager = FindAnyObjectByType<StateManager>();
        dialogue = FindAnyObjectByType<DialogueController>();
        choiceController = FindAnyObjectByType<ChoiceController>();
    }

    void Update()
    {
        if (timeManager.day > currentDay && containsCrop)
        {
            AdvanceDay();
        }
    }

    public void soilInteraction()
    {
        if (containsCrop == false)
        {
            Dictionary<ItemData, int> availableCrops = inventoryManager.queryByCategory(ItemData.Category.Crop);
            if (availableCrops.Count <= 0)
            {
                Debug.Log("You have no crops...");
                dialogue.StartDialogue(new List<string> {"You have no crops..."});
            }
            else
            {
                choiceController.StartItemSelection(availableCrops, OnCropChosen);
            }
        }
        else
        {
            if (crop.harvestTime <= daysSincePlanted)
            {
                harvestCrop();
                stateManager.soil -= 10.0f;
                Debug.Log("Soil Level: " + stateManager.soil);
            }
            else
            {
                dialogue.StartDialogue(new List<string> {"A crop is already growing here"});
            }
        }
    }

    void OnCropChosen(ItemData chosenCrop)
    {
        crop = chosenCrop;
        PlantCrop();
    }
    void PlantCrop()
    {
        inventoryManager.removeItem(crop, 1);
        containsCrop = true;
        daysSincePlanted = 0;
        currentDay = timeManager.day;
        dialogue.StartDialogue(new List<string> {"Crop has been planted!"});
    }

    void AdvanceDay()
    {
        daysSincePlanted++;
        currentDay = timeManager.day;
    }

    void harvestCrop()
    {
        dialogue.StartDialogue(new List<string> { crop.itemName + "(" + crop.baseYield + ")" + " has been harvested!" });
        inventoryManager.addItem(crop, crop.baseYield);
        daysSincePlanted = 0;
        containsCrop = false;
        crop = null;
    }
}
