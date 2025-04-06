using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> poolQueue = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            var obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    // 获取对象
    public GameObject Get()
    {
        if (poolQueue.Count > 0)
        {
            var obj = poolQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            var obj = GameObject.Instantiate(prefab, parent);
            return obj;
        }
    }

    // 回收对象
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
