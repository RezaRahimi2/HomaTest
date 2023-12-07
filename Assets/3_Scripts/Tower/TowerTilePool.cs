using System.Collections.Generic;
using UnityEngine;

public class TowerTilePool
{
    public TowerTile TilePrefab;
    private Queue<TowerTile> tilePool = new Queue<TowerTile>();
    private Transform poolParent; // Parent object to organize pooled tiles in the hierarchy

    public TowerTile GetTile()
    {
        if (tilePool.Count > 0)
        {
            TowerTile tile = tilePool.Dequeue();
            tile.gameObject.SetActive(true);
            return tile;
        }
        else
        {
            // If the pool is empty, instantiate a new tile
            return GameObject.Instantiate<TowerTile>(TilePrefab);
        }
    }

    public void ReturnTileToPool(TowerTile tile)
    {
        tile.gameObject.SetActive(false);
        tilePool.Enqueue(tile);
    }

    public void ExtendPool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            TowerTile tile = GameObject.Instantiate<TowerTile>(TilePrefab);
            ReturnTileToPool(tile);
            tile.transform.SetParent(poolParent);
        }
    }

    public int GetCurrentPoolSize()
    {
        return tilePool.Count;
    }

    public void CreatePool(int initialSize)
    {
        if (poolParent == null)
        {
            poolParent = new GameObject("TilePool").transform;
        }

        for (int i = 0; i < initialSize; i++)
        {
            TowerTile tile = GameObject.Instantiate<TowerTile>(TilePrefab);
            ReturnTileToPool(tile);
            tile.transform.SetParent(poolParent);
        }
    }
}