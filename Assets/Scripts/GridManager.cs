using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
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
        int x = Mathf.FloorToInt(local.x);
        int z = Mathf.FloorToInt(local.z);
        return new Vector2Int(x, z);
    }

    public Vector3 CellToWorld(Vector2Int coordinates, float y)
    {
        // Option 1 origin = bottom-left corner of cell (0,0)
        // Add 0.5f so we return the CENTER of the cell (most prefabs have centered pivots).
        return new Vector3(
            origin.x + coordinates.x + 0.5f,
            y,
            origin.z + coordinates.y + 0.5f
        );
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
        //Creates a height x width amount of GridCells
        cell = new GridCell[height,width];

        //Makes everything grass (for now)
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
        string StartingEdge = PickRandomWorldEdge("");
        string EndingEdge = PickRandomWorldEdge(StartingEdge);
        Vector2Int StartingCoordinates = new Vector2Int();
        Vector2Int EndingCoordinates = new Vector2Int();
        System.Random random = new System.Random();

        if(StartingEdge == "North")
        {
            StartingCoordinates = new Vector2Int(random.Next(topLeft.x + (int)quadrantSides + 1, topRight.x - (int)quadrantSides - 1), height - 1);
        }
        else if(StartingEdge == "East")
        {
            StartingCoordinates = new Vector2Int(width - 1, random.Next(bottomRight.y + (int)quadrantSides + 1, topRight.y - (int)quadrantSides - 1));
        }
        else if(StartingEdge == "South")
        {
            StartingCoordinates = new Vector2Int(random.Next(bottomLeft.x + (int)quadrantSides + 1, bottomRight.x - (int)quadrantSides - 1), 0);
        }
        else if(StartingEdge == "West")
        {
            StartingCoordinates = new Vector2Int(0, random.Next(bottomLeft.y + (int)quadrantSides + 1, topLeft.y - (int)quadrantSides - 1));
        }

        if(EndingEdge == "North")
        {
            EndingCoordinates = new Vector2Int(random.Next(topLeft.x + (int)quadrantSides + 1, topRight.x - (int)quadrantSides - 1), height - 1);
        }
        else if(EndingEdge == "East")
        {
            EndingCoordinates = new Vector2Int(width - 1, random.Next(bottomRight.y + (int)quadrantSides + 1,topRight.y - (int)quadrantSides - 1));
        }
        else if(EndingEdge == "South")
        {
            EndingCoordinates = new Vector2Int(random.Next(bottomLeft.x + (int)quadrantSides + 1, bottomRight.x - (int)quadrantSides - 1), 0);
        }
        else if(EndingEdge == "West")
        {
            EndingCoordinates = new Vector2Int(0, random.Next(bottomLeft.y + (int)quadrantSides + 1, topLeft.y - (int)quadrantSides - 1));
        }

        MakePath(StartingCoordinates, EndingCoordinates, quadrantSides);
    }

    private void MakePath(Vector2Int StartingCoordinates, Vector2Int EndingCoordinates, float quadrantSides)
    {
        int currentDistance = Manhattan(StartingCoordinates, EndingCoordinates);
        Debug.Log("Start: " + StartingCoordinates.x.ToString() + " " + StartingCoordinates.y.ToString());
        Debug.Log("End: " + EndingCoordinates.x.ToString() + " " + EndingCoordinates.y.ToString());
        Debug.Log("Distance Away: " + currentDistance);
        List<Vector2Int> steps = new List<Vector2Int>(currentDistance);
        Debug.Log("List size: " + steps.Capacity);
        steps.Add(StartingCoordinates);
        cell[StartingCoordinates.x, StartingCoordinates.y].terrain = TerrainType.Water;
        cell[EndingCoordinates.x, EndingCoordinates.y].terrain = TerrainType.Water;
        for(int x = 1; x < steps.Capacity; x++)
        {
            Vector2Int[] possibleNextSteps = new Vector2Int[] 
            {
                new Vector2Int(steps[x-1].x + 1, steps[x-1].y),
                new Vector2Int(steps[x-1].x - 1, steps[x-1].y),
                new Vector2Int(steps[x-1].x, steps[x-1].y + 1),
                new Vector2Int(steps[x-1].x, steps[x-1].y - 1)
            };
            foreach(Vector2Int step in possibleNextSteps)
            {
                if (!steps.Contains(step) && Manhattan(step, EndingCoordinates) < currentDistance && ForbiddenZoneCheck(step, quadrantSides))
                {
                    currentDistance = Manhattan(step, EndingCoordinates);
                    steps.Insert(x, step);
                }
            }
            Debug.Log("Step " + x.ToString() + " = " + steps[x].x.ToString() + steps[x].y.ToString());
            cell[steps[x].x, steps[x].y].terrain = TerrainType.Water;
        }
    }

    private bool ForbiddenZoneCheck(Vector2Int coordinates, float quadrantSides)
    {
        return  !(coordinates.x < bottomLeft.x + quadrantSides && coordinates.y < bottomLeft.y + quadrantSides) &&
                !(coordinates.x > bottomRight.x - quadrantSides && coordinates.y < bottomRight.y + quadrantSides) &&
                !(coordinates.x < topLeft.x + quadrantSides && coordinates.y > topLeft.y - quadrantSides) &&
                !(coordinates.x > topRight.x - quadrantSides && coordinates.y > topRight.y - quadrantSides) &&
                !(coordinates.x > width - 1) &&
                !(coordinates.y > height - 1) &&
                !cell[coordinates.x, coordinates.y].isBlocked;
    }

    private int Manhattan(Vector2Int StartingPoint, Vector2Int EndingPoint)
    {
        return Math.Abs(StartingPoint.x - EndingPoint.x) + Math.Abs(StartingPoint.y - EndingPoint.y);
    }

    private string PickRandomWorldEdge(string chosen)
    {
        List<string> edges = new List<string> {"North", "East", "South", "West"};
        edges.Remove(chosen);
        System.Random random = new System.Random();
        return edges[random.Next(0, edges.Count)];
    }

    private void RenderWorld()
    {
        Instantiate(grass, new Vector3(0,0,0), Quaternion.identity).transform.localScale = new Vector3(width, 1, height);

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
