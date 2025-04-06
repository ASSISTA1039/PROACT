using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
    public GameObject spherePrefab; // 球体预制体
    public Transform spawnParent; // 球体的父节点
    public int maxSpheres = 3; // 场上最多存在的球体数量


    [SerializeField, Header("敌人检测")] private float detectionRang;
    [SerializeField] protected LayerMask enemyLayer;
    //固定对象BOSS
    [SerializeField] public Collider[] BOSSTarget;

    private ObjectPool spherePool;
    public Queue<GameObject> activeSpheres = new Queue<GameObject>();

    void Start()
    {
        // 初始化对象池
        spherePool = new ObjectPool(spherePrefab, maxSpheres, spawnParent);
    }
    private void Update()
    {
        foreach (var sphere in activeSpheres)
        {
            if (sphere.activeInHierarchy) // 确保球体是激活状态
            {
                int targetCount = Physics.OverlapSphereNonAlloc(gameObject.transform.position, detectionRang, BOSSTarget, enemyLayer);
                if (targetCount > 0)
                {

                }
            }
        }

    }
    public void SpawnSphere(Vector3 position)
    {
        // 如果球体数量达到上限，回收最早的球体
        if (activeSpheres.Count >= maxSpheres)
        {
            var oldSphere = activeSpheres.Dequeue();
            spherePool.ReturnToPool(oldSphere);
        }

        // 从对象池获取新球体
        var newSphere = spherePool.Get();
        newSphere.transform.position = position;
        activeSpheres.Enqueue(newSphere);
    }
}
