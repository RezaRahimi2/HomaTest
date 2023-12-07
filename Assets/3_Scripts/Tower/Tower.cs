using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tower : MonoBehaviour
{
    [Header("Tower Settings")] public float TileHeight = 1.2f;
    public float TileRadius = 0.5f;
    public int TileCountPerFloor = 15;
    public int FloorCount = 15;
    public int PlayableFloors = 8;
    public float SpecialTileChance = 0.1f;
    public TowerTile[] SpecialTilePrefabs;
    public bool BuildOnStart = true;
    private TowerTile m_tilePrefab;

    
    [Header("Scene")] public Transform CameraTarget;

    private List<List<TowerTile>> tilesByFloor;
    private int currentFloor = -1;
    private int maxFloor = 0;

    public System.Action<TowerTile> OnTileDestroyedCallback;
    

    public float CaculateTowerRadius(float sideLength, float sideCount)
    {
        return sideLength / (2 * Mathf.Sin(Mathf.Deg2Rad * (180.0f / sideCount)));
    }

    public void BuildTower()
    {
        m_tilePrefab = PrefabManager.Instance.TilePrefab;
        ResetTower();
        tilesByFloor = new List<List<TowerTile>>();
        float towerRadius = CaculateTowerRadius(TileRadius * 2, TileCountPerFloor);
        float angleStep = 360.0f / TileCountPerFloor;
        Quaternion floorRotation = transform.rotation;

        int totalTilesNeeded = FloorCount * TileCountPerFloor;

        if (Application.isPlaying)
        {
            // Check if the total tiles needed exceed the current pool size
            if (totalTilesNeeded > PoolingSystemManager.Instance.GetOrCreatePool(m_tilePrefab).GetCurrentPoolSize())
            {
                int tilesToAdd = totalTilesNeeded -
                                 PoolingSystemManager.Instance.GetOrCreatePool(m_tilePrefab).GetCurrentPoolSize();
                PoolingSystemManager.Instance.GetOrCreatePool(m_tilePrefab, tilesToAdd)
                    .EnsureQuantity(m_tilePrefab, tilesToAdd);
            }
        }

        for (int y = 0; y < FloorCount; y++)
        {
            tilesByFloor.Add(new List<TowerTile>());
            for (int i = 0; i < TileCountPerFloor; i++)
            {
                Quaternion direction = Quaternion.AngleAxis(angleStep * i, Vector3.up) * floorRotation;
                Vector3 position = transform.position + Vector3.up * y * TileHeight +
                                   direction * Vector3.forward * towerRadius;
                
                // only use pooling system in play mode to be able build tower in editor
                TowerTile tileInstance = Application.isPlaying?
                    PoolingSystemManager.Instance.GetPooledObject(Random.value > SpecialTileChance ? 
                        m_tilePrefab : SpecialTilePrefabs[Random.Range(0, SpecialTilePrefabs.Length)],
                        Vector3.zero, Quaternion.identity):Instantiate(Random.value > SpecialTileChance ? 
                    m_tilePrefab : SpecialTilePrefabs[Random.Range(0, SpecialTilePrefabs.Length)], position, direction * m_tilePrefab.transform.rotation, transform);;
                

                tileInstance.transform.position = position;
                tileInstance.transform.rotation = direction * m_tilePrefab.transform.rotation;

                tileInstance.SetColorIndex(Mathf.FloorToInt(Random.value * TileColorManager.Instance.ColorCount));
                tileInstance.SetFreezed(true);
                tileInstance.Floor = y;
                tileInstance.OnTileDestroyed += OnTileDestroyedCallback;
                tileInstance.OnTileDestroyed += OnTileDestroyed;
                tilesByFloor[y].Add(tileInstance);
            }

            floorRotation *= Quaternion.AngleAxis(angleStep / 2.0f, Vector3.up);
        }

        maxFloor = FloorCount - 1;

        SetCurrentFloor(tilesByFloor.Count - PlayableFloors);
        for (int i = 1; i < PlayableFloors; i++)
        {
            SetFloorActive(currentFloor + i, true);
        }
    }


    public void OnTileDestroyed(TowerTile tile)
    {
        if (maxFloor > PlayableFloors - 1 && tilesByFloor != null)
        {
            tilesByFloor[tile.Floor].Remove(tile);
            float checkHeight = (maxFloor - 1) * TileHeight + TileHeight * 0.9f;
            float maxHeight = 0;
            foreach (List<TowerTile> floor in tilesByFloor)
            {
                foreach (TowerTile t in floor)
                {
                    if (t != null)
                        maxHeight = Mathf.Max(t.transform.position.y, maxHeight);
                }
            }

            if (maxHeight < checkHeight)
            {
                maxFloor--;
                if (currentFloor > 0)
                {
                    SetCurrentFloor(currentFloor - 1);
                }
            }
        }
    }

    public void ResetTower()
    {
        if (tilesByFloor != null)
        {
            foreach (List<TowerTile> tileList in tilesByFloor)
            {
                foreach (TowerTile tile in tileList)
                {
                    if (Application.isPlaying)
                        PoolingSystemManager.Instance.GetPool<TowerTile>().ReturnToPool(tile);
                    else
                        DestroyImmediate(tile.gameObject);
                }

                tileList.Clear();
            }

            tilesByFloor.Clear();
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        for (int i = 0; i < tilesByFloor.Count - PlayableFloors; i++)
        {
            yield return new WaitForSeconds(0.075f * Time.timeScale);
            SetFloorActive(i, false, false);
        }

        yield return null;
    }

    public void SetCurrentFloor(int floor)
    {
        currentFloor = floor;
        CameraTarget.position = transform.position + Vector3.up * floor * TileHeight;
        SetFloorActive(currentFloor, true);
    }

    public void SetFloorActive(int floor, bool value, bool setFreezed = true)
    {
        foreach (TowerTile tile in tilesByFloor[floor])
        {
            if (tile && tile.isActiveAndEnabled)
            {
                tile.SetEnabled(value);
                if (setFreezed)
                    tile.SetFreezed(!value);
            }
        }
    }
}