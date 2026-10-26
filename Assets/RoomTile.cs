using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField] private GameObject wallPosX;
    [SerializeField] private GameObject wallNegX;
    [SerializeField] private GameObject wallPosZ;
    [SerializeField] private GameObject wallNegZ;

    [Header("Grid")]
    [SerializeField] private float gridSize = 1f;

    // Evidence vöech budov podle jejich pozice.
    private static readonly HashSet<Vector2Int> occupiedCells =
        new HashSet<Vector2Int>();

    // Evidence pouze chodeb, jejichû zdi aktualizujeme.
    private static readonly HashSet<RoomTile> hallways =
        new HashSet<RoomTile>();

    private Vector2Int cell;
    private bool isPreview;
    private bool isRegistered;

    public void SetPreviewMode(bool value)
    {
        isPreview = value;
    }

    private void Start()
    {
        if (isPreview)
            return;

        cell = GetCell(transform.position);

        // Registrujeme tuto chodbu.
        hallways.Add(this);
        occupiedCells.Add(cell);
        isRegistered = true;

        RefreshAllHallways();
    }

    private void OnDestroy()
    {
        if (isPreview || !isRegistered)
            return;

        hallways.Remove(this);
        occupiedCells.Remove(cell);

        RefreshAllHallways();
    }

    // Vol· BuildingSystem po postavenÌ jakÈkoliv budovy.
    public static void NotifyBuildingPlaced(Vector3 position, float size = 1f)
    {
        Vector2Int placedCell = GetCellStatic(position, size);

        occupiedCells.Add(placedCell);

        RefreshAllHallways();
    }

    private Vector2Int GetCell(Vector3 position)
    {
        return GetCellStatic(position, gridSize);
    }

    private static Vector2Int GetCellStatic(Vector3 position, float size)
    {
        return new Vector2Int(
            Mathf.RoundToInt(position.x / size),
            Mathf.RoundToInt(position.z / size)
        );
    }

    private static void RefreshAllHallways()
    {
        foreach (RoomTile hallway in hallways)
        {
            if (hallway != null)
                hallway.UpdateWalls();
        }
    }

    private void UpdateWalls()
    {
        // Zjiöùujeme svÏtovÈ smÏry podle aktu·lnÌ rotace chodby.
        Vector2Int right = GetWorldDirection(Vector3.right);
        Vector2Int left = GetWorldDirection(Vector3.left);
        Vector2Int forward = GetWorldDirection(Vector3.forward);
        Vector2Int back = GetWorldDirection(Vector3.back);

        SetWall(wallPosX, !HasBuilding(right));
        SetWall(wallNegX, !HasBuilding(left));
        SetWall(wallPosZ, !HasBuilding(forward));
        SetWall(wallNegZ, !HasBuilding(back));
    }

    private Vector2Int GetWorldDirection(Vector3 localDirection)
    {
        Vector3 direction = transform.TransformDirection(localDirection);

        return new Vector2Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.z)
        );
    }

    private bool HasBuilding(Vector2Int direction)
    {
        Vector2Int neighborCell = cell + direction;

        return occupiedCells.Contains(neighborCell);
    }

    private void SetWall(GameObject wall, bool visible)
    {
        if (wall != null && wall.activeSelf != visible)
            wall.SetActive(visible);
    }
}