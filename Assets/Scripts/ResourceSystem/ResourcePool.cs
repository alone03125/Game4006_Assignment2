using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    public static ResourcePool Instance;

    [System.Serializable]
    public class ResourcePrefab
    {
        public ResourceType type;
        public GameObject prefab;
    }

    public List<ResourcePrefab> resourcePrefabs = new List<ResourcePrefab>();
    public Terrain terrain;
    public int initnum = 20;

    private Dictionary<ResourceType, Queue<GameObject>> pool = new Dictionary<ResourceType, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化对象池
        foreach (var typePref in resourcePrefabs)
        {
            pool[typePref.type] = new Queue<GameObject>();

            for (int i = 0; i < initnum; i++)
            {
                GameObject obj = Instantiate(typePref.prefab);
                obj.SetActive(false);
                // 确保初始状态重置
                var node = obj.GetComponent<ResourceNode>();
                if (node != null) node.ResetNode();
                pool[typePref.type].Enqueue(obj);
            }
        }

        // 初始化场景中的资源节点
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < initnum / 2; j += 1)
            {
                GetResource((ResourceType)i);
            }
        }
    }

    /// <summary>
    /// 从对象池获取资源，如果pool中没有则创建新的
    /// </summary>
    public GameObject GetResource(ResourceType type)
    {
        GameObject obj;

        if (pool[type].Count > 0)
        {
            obj = pool[type].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(resourcePrefabs.Find(x => x.type == type).prefab);
        }

        // 随机位置放置
        Vector3 pos = GetRandomPositionOnTerrain(type);
        obj.transform.position = pos;
        obj.name = type.ToString() + "_Node";

        // 重置节点状态
        var node = obj.GetComponent<ResourceNode>();
        if (node != null) node.ResetNode();

        return obj;
    }

    /// <summary>
    /// 将资源对象返回对象池
    /// </summary>
    public void ReturnResource(ResourceType type, GameObject obj)
    {
        obj.SetActive(false);
        pool[type].Enqueue(obj);
    }

    /// <summary>
    /// 获取Terrain上的随机位置，确保高度合适
    /// Crop和Stone资源额外上升0.5格
    /// </summary>
    private Vector3 GetRandomPositionOnTerrain(ResourceType type)
    {
        Vector3 terrainPos = terrain.GetPosition();
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        float randomX = terrainPos.x + Random.Range(60f, terrainWidth - 40f);
        float randomZ = terrainPos.z + Random.Range(50f, terrainLength - 50f);

        float height = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));
        float yOffset = 0f;

        if (type == ResourceType.Crop || type == ResourceType.Stone)
        {
            yOffset += 0.5f;
        }

        return new Vector3(randomX, terrainPos.y + height + yOffset, randomZ);
    }
}