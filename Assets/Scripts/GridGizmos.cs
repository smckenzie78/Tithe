using UnityEngine;

public class GridGizmos : MonoBehaviour
{
    [Header("Grid Dimensions")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    [Header("Grid Settings")]
    [SerializeField] private Vector3 origin = new Vector3(-32, 0, -32); // bottom-left corner of cell (0,0)
    [SerializeField] private float cellSize = 1f;

    [Header("Gizmo Settings")]
    [SerializeField] private bool drawGrid = true;
    [SerializeField] private bool drawBorder = true;
    [SerializeField] private bool drawOriginMarker = true;

    [Tooltip("Raise the gizmo lines slightly so they don't z-fight with the ground.")]
    [SerializeField] private float yOffset = 0.02f;

    private void OnDrawGizmos()
    {
        if (!drawGrid && !drawBorder && !drawOriginMarker) return;

        float y = origin.y + yOffset;

        if (drawOriginMarker)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(new Vector3(origin.x, y, origin.z), 0.15f);
        }

        if (drawBorder)
        {
            Gizmos.color = Color.yellow;

            Vector3 bl = new Vector3(origin.x, y, origin.z);
            Vector3 br = new Vector3(origin.x + width * cellSize, y, origin.z);
            Vector3 tl = new Vector3(origin.x, y, origin.z + height * cellSize);
            Vector3 tr = new Vector3(origin.x + width * cellSize, y, origin.z + height * cellSize);

            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tr, tl);
            Gizmos.DrawLine(tl, bl);
        }

        if (!drawGrid) return;

        Gizmos.color = new Color(1f, 1f, 1f, 0.25f);

        // Draw vertical grid lines
        for (int x = 0; x <= width; x++)
        {
            float wx = origin.x + x * cellSize;
            Vector3 a = new Vector3(wx, y, origin.z);
            Vector3 b = new Vector3(wx, y, origin.z + height * cellSize);
            Gizmos.DrawLine(a, b);
        }

        // Draw horizontal grid lines
        for (int z = 0; z <= height; z++)
        {
            float wz = origin.z + z * cellSize;
            Vector3 a = new Vector3(origin.x, y, wz);
            Vector3 b = new Vector3(origin.x + width * cellSize, y, wz);
            Gizmos.DrawLine(a, b);
        }
    }
}
