using UnityEngine;

public enum TerrainType : byte
{
    Grass = 0,
    Soil  = 1,
    Water = 2,
    Sand  = 3,
    Rock  = 4
}
public class GridCell
{
    public TerrainType terrain;
    public ItemData crop;
    public int daysSincePlanted;
    public bool isBlocked;
}
