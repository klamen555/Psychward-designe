using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingSystem : MonoBehaviour
{
    public GameObject buildingPrefab;
    public LayerMask groundLayer;

    private GameObject preview;
    private bool isBuilding = false;

    private HashSet<Vector2Int> occupiedCells =
        new HashSet<Vector2Int>();

    private float gridSize = 1f;

    void Update()
    {
        // B zapne stavìní
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartBuilding();
        }

        if (!isBuilding)
            return;

        // Otoèení místnosti o 90 stupòù
        if (Input.GetKeyDown(KeyCode.R))
        {
            preview.transform.Rotate(0f, 90f, 0f);
        }

        UpdatePreview();

        // Levé tlaèítko myši postaví budovu
        // Kliknutí na UI nesmí postavit budovu
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            PlaceBuilding();
        }

        // Pravé tlaèítko nebo Escape stavìní zruší
        if (Input.GetMouseButtonDown(1) ||
            Input.GetKeyDown(KeyCode.Escape))
        {
            CancelBuilding();
        }
    }


    public void SelectBuilding(GameObject prefab)
    {
        if (prefab == null)
            return;

        buildingPrefab = prefab;

        StartBuilding();
    }


    void StartBuilding()
    {


        if (buildingPrefab == null)
        {
            Debug.LogWarning("Není vybrán žádný prefab budovy!");
            return;
        }

        if (preview != null)
            Destroy(preview);

        preview = Instantiate(buildingPrefab);
        isBuilding = true;

        SetPreviewColor(Color.green);
    }

    void UpdatePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(
            Input.mousePosition
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 100f,
            groundLayer))
        {
            Vector2Int cell = GetCell(hit.point);

            preview.transform.position = new Vector3(
                cell.x * gridSize,
                0.5f,
                cell.y * gridSize
            );

            bool canBuild = CanBuild(cell);

            SetPreviewColor(
                canBuild ? Color.green : Color.red
            );
        }
    }

    Vector2Int GetCell(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / gridSize);
        int z = Mathf.RoundToInt(position.z / gridSize);

        return new Vector2Int(x, z);
    }


    bool CanBuild(Vector2Int cell)
    {
        // Nedovolíme stavìt na obsazeném políèku.
        if (occupiedCells.Contains(cell))
            return false;

        return true;
    }


    void PlaceBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            return;

        Vector2Int cell = GetCell(hit.point);

        if (!CanBuild(cell))
            return;

        Vector3 position = new Vector3(
            cell.x * gridSize,
            0.5f,
            cell.y * gridSize
        );

        GameObject placedBuilding = Instantiate(
            buildingPrefab,
            position,
            preview.transform.rotation
        );

        // Zaznamenáme každou budovu, nejen chodbu.
        occupiedCells.Add(cell);

        // Aktualizujeme zdi všech existujících chodeb.
        RoomTile.NotifyBuildingPlaced(placedBuilding.transform.position, gridSize);
    }

    void SetPreviewColor(Color color)
    {
        if (preview == null)
            return;

        Renderer[] renderers =
            preview.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    void CancelBuilding()
    {
        if (preview != null)
            Destroy(preview);

        preview = null;
        isBuilding = false;
    }
}

