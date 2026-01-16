using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public ItemData crop;
    InventoryManager inventoryManager;
    TimeManager timeManager;
    StateManager stateManager;
    DialogueController dialogue;
    ChoiceController choiceController;
    public ItemData[,] plantedCrop = new ItemData[64,64];
    public int[,] daysSincePlanted = new int[64,64];
    private float currentDay;
    public Vector3 origin = new Vector3(-32, 0, -32);
    int[,] blockerId = new int[64,64];
    public enum TerrainType : byte
    {
        Grass = 0,
        Soil  = 1,
        Water = 2,
        Sand  = 3,
        Rock  = 4
    }
    private TerrainType[,] terrain;

    void Awake()
    {
        terrain = new TerrainType[64, 64];

        // For now, initialize everything to grass (until procedural gen fills it)
        for (int x = 0; x < 64; x++)
            for (int z = 0; z < 64; z++)
                terrain[x, z] = TerrainType.Grass;

        terrain[30, 32] = TerrainType.Soil;
        terrain[32, 32] = TerrainType.Soil;
        terrain[34, 32] = TerrainType.Soil;

        inventoryManager = FindAnyObjectByType<InventoryManager>();
        timeManager = FindAnyObjectByType<TimeManager>();
        stateManager = FindAnyObjectByType<StateManager>();
        dialogue = FindAnyObjectByType<DialogueController>();
        choiceController = FindAnyObjectByType<ChoiceController>();
    }

    void Start()
    {
        for (int x = 0; x < 64; x++)
            for (int z = 0; z < 64; z++)
                if(terrain[x,z] != TerrainType.Grass)
                {
                    blockerId[x,z] = 1;
                }
    }
    void Update()
    {
        
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 local = worldPos - origin;
        int x = Mathf.RoundToInt(local.x);
        int z = Mathf.RoundToInt(local.z);
        return new Vector2Int(x, z);
    }

    public bool IsOccupied(Vector2Int cell)
    {
        return blockerId[cell.x, cell.y] != 0;
    }

    public int GetTerrainType(Vector2Int cell)
    {
        if(terrain[cell.x, cell.y] == TerrainType.Soil)
        {
            return 1;
        }
        else return 0;
    }

    //Grid cells logic for crops begins
    public bool ContainsCrop(Vector2Int cell)
    {
        return plantedCrop[cell.x, cell.y] != null && plantedCrop[cell.x, cell.y].category == ItemData.Category.Crop;
    }

    public void TryPlantingCrop(Vector2Int cell)
    {
        if (ContainsCrop(cell) == false)
        {
            Dictionary<ItemData, int> availableCrops = inventoryManager.queryByCategory(ItemData.Category.Crop);
            if (availableCrops.Count <= 0)
            {
                Debug.Log("You have no crops...");
                dialogue.StartDialogue(new List<string> {"You have no crops..."});
            }
            else
            {
                choiceController.StartItemSelection(availableCrops, (ItemData chosenCrop) => 
                    {
                        inventoryManager.removeItem(chosenCrop, 1);
                        plantedCrop[cell.x, cell.y] = chosenCrop;
                        daysSincePlanted[cell.x, cell.y] = 0;
                        dialogue.StartDialogue(new List<string> {"Crop has been planted!"});
                    }
                );
            }
        }
        else
        {
            if (plantedCrop[cell.x, cell.y].harvestTime <= daysSincePlanted[cell.x, cell.y])
            {
                HarvestCrop(cell);
                stateManager.soil -= 10.0f;
                Debug.Log("Soil Level: " + stateManager.soil);
            }
            else
            {
                dialogue.StartDialogue(new List<string> {"A crop is already growing here"});
            }
        }
    }

    void HarvestCrop(Vector2Int cell)
    {
        dialogue.StartDialogue(new List<string> { plantedCrop[cell.x, cell.y].itemName + "(" + plantedCrop[cell.x, cell.y].baseYield + ")" + " has been harvested!" });
        inventoryManager.addItem(plantedCrop[cell.x, cell.y], plantedCrop[cell.x, cell.y].baseYield);
        daysSincePlanted[cell.x, cell.y] = 0;
        plantedCrop[cell.x, cell.y] = null;
    }

    public void OnDayAdvanced()
    {
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                if (plantedCrop[x, y] != null) // crop exists
                {
                    daysSincePlanted[x, y]++;
                }
            }
        }
    }

    //Grid cells logic for crops ends
}
