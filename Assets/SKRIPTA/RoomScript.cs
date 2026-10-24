
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

    private Vector2Int cell;

    private static Dictionary<Vector2Int, RoomTile> rooms =
        new Dictionary<Vector2Int, RoomTile>();

    private static readonly Vector2Int right =
        new Vector2Int(1, 0);

    private static readonly Vector2Int left =
        new Vector2Int(-1, 0);

    private static readonly Vector2Int forward =
        new Vector2Int(0, 1);

    private static readonly Vector2Int back =
        new Vector2Int(0, -1);

    private void OnEnable()
    {
        RegisterRoom();
    }

    private void Start()
    {
        RefreshAllRooms();
    }

    private void OnDisable()
    {
        // Odstraníme místnost z registru,
        // pouze pokud tam stále patøí tento objekt.
        if (rooms.TryGetValue(cell, out RoomTile room)
            && room == this)
        {
            rooms.Remove(cell);
        }

        RefreshAllRooms();
    }

    private void RegisterRoom()
    {
        cell = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / gridSize),
            Mathf.RoundToInt(transform.position.z / gridSize)
        );

        if (rooms.TryGetValue(cell, out RoomTile existing)
            && existing != null && existing != this)
        {
            Debug.LogWarning(
                "Na políèku " + cell +
                " už je zaregistrovaná jiná místnost!", this
            );
            return;
        }

        rooms[cell] = this;
        RefreshAllRooms();
    }

    private void RefreshAllRooms()
    {
        // Aktualizujeme stìny všech existujících místností.
        foreach (RoomTile room in rooms.Values)
        {
            if (room != null)
                room.UpdateWalls();
        }
    }

    private void UpdateWalls()
    {
        SetWall(wallPosX, !HasRoom(right));
        SetWall(wallNegX, !HasRoom(left));
        SetWall(wallPosZ, !HasRoom(forward));
        SetWall(wallNegZ, !HasRoom(back));
    }

    private bool HasRoom(Vector2Int direction)
    {
        Vector2Int neighborCell = cell + direction;

        return rooms.TryGetValue(
            neighborCell, out RoomTile neighbor
        ) && neighbor != null && neighbor != this;
    }

    private void SetWall(GameObject wall, bool visible)
    {
        if (wall != null && wall.activeSelf != visible)
        {
            wall.SetActive(visible);
        }
    }
}
