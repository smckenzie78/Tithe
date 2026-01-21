using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int height;
    [SerializeField] private int width;
    public GameObject grass;
    public GameObject soil;
    public GameObject water;
    Vector2Int bottomLeft;
    Vector2Int bottomRight;
    Vector2Int topLeft;
    Vector2Int topRight;
    InventoryManager inventoryManager;
    TimeManager timeManager;
    StateManager stateManager;
    DialogueController dialogue;
    ChoiceController choiceController;
    private GridCell[,] cell;
    public Vector3 origin = new Vector3(-32, 0, -32);

    void Awake()
    {
        bottomLeft = new Vector2Int(0,0);
        bottomRight = new Vector2Int(width,0);
        topLeft = new Vector2Int(0,height);
        topRight = new Vector2Int(width,height);

        inventoryManager = FindAnyObjectByType<InventoryManager>();
        timeManager = FindAnyObjectByType<TimeManager>();
        stateManager = FindAnyObjectByType<StateManager>();
        dialogue = FindAnyObjectByType<DialogueController>();
        choiceController = FindAnyObjectByType<ChoiceController>();

        GenerateWorld();
    }

    void Start()
    {
        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                if(cell[x, z].terrain != TerrainType.Grass)
                {
                    cell[x,z].isBlocked = true;
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

    public Vector3 CellToWorld(Vector2Int coordinates, float y)
    {
        return new Vector3(coordinates.x + origin.x, y, coordinates.y + origin.z);
    }

    public bool IsOccupied(Vector2Int coordinates)
    {
        return cell[coordinates.x, coordinates.y].isBlocked;
    }

    public int GetTerrainType(Vector2Int coordinates)
    {
        if(cell[coordinates.x, coordinates.y].terrain == TerrainType.Soil)
        {
            return 1;
        }
        else return 0;
    }

    //Grid cells logic for crops begins
    public bool ContainsCrop(Vector2Int coordinates)
    {
        return cell[coordinates.x, coordinates.y].crop != null && cell[coordinates.x, coordinates.y].crop.category == ItemData.Category.Crop;
    }

    public void TryPlantingCrop(Vector2Int coordinates)
    {
        if (ContainsCrop(coordinates) == false)
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
                        cell[coordinates.x, coordinates.y].crop = chosenCrop;
                        cell[coordinates.x, coordinates.y].daysSincePlanted = 0;
                        dialogue.StartDialogue(new List<string> {"Crop has been planted!"});
                    }
                );
            }
        }
        else
        {
            if (cell[coordinates.x, coordinates.y].crop.harvestTime <= cell[coordinates.x, coordinates.y].daysSincePlanted)
            {
                HarvestCrop(coordinates);
                stateManager.soil -= 10.0f;
                Debug.Log("Soil Level: " + stateManager.soil);
            }
            else
            {
                dialogue.StartDialogue(new List<string> {"A crop is already growing here"});
            }
        }
    }

    void HarvestCrop(Vector2Int coordinates)
    {
        dialogue.StartDialogue(new List<string> { cell[coordinates.x, coordinates.y].crop.itemName + "(" + cell[coordinates.x, coordinates.y].crop.baseYield + ")" + " has been harvested!" });
        inventoryManager.addItem(cell[coordinates.x, coordinates.y].crop, cell[coordinates.x, coordinates.y].crop.baseYield);
        cell[coordinates.x, coordinates.y].daysSincePlanted = 0;
        cell[coordinates.x, coordinates.y].crop = null;
    }

    public void OnDayAdvanced()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cell[x, y].crop != null) // crop exists
                {
                    cell[x, y].daysSincePlanted++;
                }
            }
        }
    }
    //Grid cells logic for crops ends

    private void GenerateWorld()
    {
        cell = new GridCell[height,width];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                cell[x, z] = new GridCell
                {
                    terrain = TerrainType.Grass,
                };
            }
        }

        //Splits world in half, then slices one half into 4 pieces. This area is where no water will be allowed.
        int quadrantArea = width * height / 2 / 4;
        //These quadrants should be perfect squares, so taking the sqrt of one of them should give you the length of all sides.
        float quadrantSides = Mathf.Sqrt(quadrantArea);
        //We decide what percentage of the "eligible for water" land should actually contain water. This will be the cell "budget."
        float percentage = 0.2f;
        float waterBudget = ((height * width) - (quadrantArea * 4)) * percentage;
        //We generate the water, passing in the necessary info.
        GenerateWater(quadrantSides, waterBudget);
 
        cell[30, 32].terrain = TerrainType.Soil;
        cell[32, 32].terrain = TerrainType.Soil;
        cell[34, 32].terrain = TerrainType.Soil;

        RenderWorld();
    }

    private void GenerateWater(float quadrantSides, float waterBudget)
    {
        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (!(x < bottomLeft.x + quadrantSides && z < bottomLeft.y + quadrantSides) &&
                !(x > bottomRight.x - quadrantSides && z < bottomRight.y + quadrantSides) &&
                !(x < topLeft.x + quadrantSides && z > topLeft.y - quadrantSides) &&
                !(x > topRight.x - quadrantSides && z > topRight.y - quadrantSides))
                {
                    if(count < waterBudget)
                    {
                        cell[x,z].terrain = TerrainType.Water;
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private void RenderWorld()
    {
        Instantiate(grass, new Vector3(0,0,0), Quaternion.identity).transform.localScale = new Vector3(width+1, 1, height+1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if(cell[x,z].terrain == TerrainType.Soil)
                {
                    Instantiate(soil, CellToWorld(new Vector2Int(x,z), soil.transform.position.y), Quaternion.identity);
                }
                else if(cell[x,z].terrain == TerrainType.Water)
                {
                    Instantiate(water, CellToWorld(new Vector2Int(x,z), soil.transform.position.y), Quaternion.identity);
                }
            }
        }
    }

}
