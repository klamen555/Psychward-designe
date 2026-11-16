
using System.Collections.Generic;
using UnityEngine;

public class PlSpawnManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cell1Prefab;
    public GameObject cell2Prefab;
    public Transform player;

    private List<Transform> spawnBuildings = new List<Transform>();

    public void RegisterBuilding(GameObject building)
    {
        string buildingName = building.name.Replace("(Clone)", "").Trim();

        if (buildingName == cell1Prefab.name ||
            buildingName == cell2Prefab.name)
        {
            spawnBuildings.Add(building.transform);

            Debug.Log("Spawn buildings: " + spawnBuildings.Count);
        }
    }

    public void RandomSpawn()
    {

        int randomIndex = Random.Range(0, spawnBuildings.Count);

        Transform selectedSpawn = spawnBuildings[randomIndex];

        player.position = selectedSpawn.position + Vector3.up;
        player.rotation = selectedSpawn.rotation;
    }
}
