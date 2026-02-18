using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject baseRoom;
    [SerializeField] private GameObject doorSpawner;
    [SerializeField] private GameObject baseDoorOpen;
    [SerializeField] private GameObject baseDoorClose;
    [SerializeField] private Vector2Int dimensions;
    private List<GameObject> clusterDoors1 = new List<GameObject>();
    private List<GameObject> clusterDoors2 = new List<GameObject>();
    private List<GameObject> clusterDoors3 = new List<GameObject>();
    private List<GameObject> clusterDoors4 = new List<GameObject>();

    public List<Rooms> roomsList;
    

    private void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        CleanEverything();
        GenerateDoorSpawnPoints();
        GenerateDoors();
        GenerateRooms();
    }

    public void GenerateDoorSpawnPoints()
    {
        float x = dimensions.x / 2f;
        float y = dimensions.y / 2f;

        CleanClusters();
            
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <=2; j++)
            {
                if((i + j)%2 == 0) continue;
                    
                var spawner = Instantiate(doorSpawner, new Vector3(i * x, 3, j * y), Quaternion.identity, transform);
                PlaceSpawnerInCluster(spawner, new Vector2(i,j));
            }
        }
    }

    private void GenerateDoors()
    {
        var r1 = Random.Range(0, 4);
        var r2 = Random.Range(0, 2);
        var r3 = Random.Range(0, 2);
        var r4 = Random.Range(0, 4);
        
        // Cluster 1

        for (int i = 0; i < clusterDoors1.Count; i++)
        {
            if(i == r1) Instantiate(baseDoorClose, clusterDoors1[i].transform.position, Quaternion.identity, transform);
            else Instantiate(baseDoorOpen, clusterDoors1[i].transform.position, Quaternion.identity, transform);
        }
        
        // Cluster 2

        for (int i = 0; i < clusterDoors2.Count; i++)
        {
            if(i == r2) Instantiate(baseDoorClose, clusterDoors2[i].transform.position, Quaternion.identity, transform);
            else Instantiate(baseDoorOpen, clusterDoors2[i].transform.position, Quaternion.identity, transform);
        }
        
        // Cluster 3

        for (int i = 0; i < clusterDoors3.Count; i++)
        {
            if(i == r3) Instantiate(baseDoorClose, clusterDoors3[i].transform.position, Quaternion.identity, transform);
            else Instantiate(baseDoorOpen, clusterDoors3[i].transform.position, Quaternion.identity, transform);
        }
        
        // Cluster 4

        for (int i = 0; i < clusterDoors4.Count; i++)
        {
            if(i == r4) Instantiate(baseDoorClose, clusterDoors4[i].transform.position, Quaternion.identity, transform);
            else Instantiate(baseDoorOpen, clusterDoors4[i].transform.position, Quaternion.identity, transform);
        }
        
    }

    private void GenerateRooms()
    {
        for (int i = -1; i <= 1; i ++)
        {
            for (int j = -1; j <= 1; j ++)
            {
                var type = GetTypeOfRoom(new Vector2Int(i, j));

                var r = Random.Range(0, roomsList[type].rooms.Count);
                GameObject roomToSpawn = roomsList[type].rooms[r];
                
                var room = Instantiate(roomToSpawn, new Vector3(i*dimensions.x, 0, j*dimensions.y), Quaternion.identity, transform);
            }
        }
    }

    private int GetTypeOfRoom(Vector2 coord)
    {
        if(coord == new Vector2(-1,-1)) return 6;
        if(coord == new Vector2(-1, 0)) return 3;
        if(coord == new Vector2(-1, 1)) return 0;
        if(coord == new Vector2(0, -1)) return 7;
        if(coord == new Vector2(0, 0)) return 4;
        if(coord == new Vector2(0, 1)) return 1;
        if(coord == new Vector2(1, -1)) return 8;
        if(coord == new Vector2(1, 0)) return 5;
        if(coord == new Vector2(1, 1)) return 2;
        
        // default
        return 0;
    }
    
    private void CleanClusters()
    {
        foreach (var room in clusterDoors1) DestroyImmediate(room);
        foreach (var room in clusterDoors2) DestroyImmediate(room);
        foreach (var room in clusterDoors3) DestroyImmediate(room);
        foreach (var room in clusterDoors4) DestroyImmediate(room);
        
        clusterDoors1 = new List<GameObject>();
        clusterDoors2 = new List<GameObject>();
        clusterDoors3 = new List<GameObject>();
        clusterDoors4 = new List<GameObject>();
    }

    private void PlaceSpawnerInCluster(GameObject spawner, Vector2 coord)
    {
        switch (coord.x)
        {
            case <= 0 when coord.y >= 0:
                clusterDoors1.Add(spawner);
                break;
            case > 0 when coord.y > 0:
                clusterDoors2.Add(spawner);
                break;
            case < 0 when coord.y < 0:
                clusterDoors3.Add(spawner);
                break;
            case >= 0 when coord.y <= 0:
                clusterDoors4.Add(spawner);
                break;
            default:
                Debug.Log("Un point n'a pas trouvé de cluster, coordoonnées : " + coord);
                break;
        }
    }

    public void CleanEverything()
    {
        var c = transform.childCount;

        for (int i = c - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

[Serializable]
public class Rooms
{
    public string name;
    public List<GameObject> rooms;
}
