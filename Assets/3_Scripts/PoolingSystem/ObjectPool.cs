using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool<T> where T : MonoBehaviour
{
    private List<T> pool = new List<T>();

    public int GetCurrentPoolSize()
    {
        return pool.Count;
    }

    public void EnsureQuantity(T prototype, int count)
    {
        for (int i = 0; i < count ; i++)
        {
            T newObj = Object.Instantiate(prototype);
            if (Application.isPlaying)
            {
                Object.DontDestroyOnLoad(newObj);
            }
            newObj.gameObject.SetActive(false);
            pool.Add(newObj);
        }
    }

    public T GetPooled(T prototype, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        T unused = pool.Find(x => !x.gameObject.activeSelf);
        if (!unused)
        {
            unused = Object.Instantiate(prototype, position, rotation, parent);
            pool.Add(unused);
            Object.DontDestroyOnLoad(unused);
        }
        else
        {
            unused.transform.position = position;
            unused.transform.rotation = rotation;
            if (unused.transform.parent != parent)
                unused.transform.parent = parent;
            unused.gameObject.SetActive(true);
        }
        return unused;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void ReturnAllToPool()
    {
        pool.ForEach(x => x.gameObject.SetActive(false));
    }
}