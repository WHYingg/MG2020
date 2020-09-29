using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 池子数据
/// </summary>
public class PoolData
{
    /// <summary>
    /// 对象挂载的父节点
    /// </summary>
    public GameObject parent;

    /// <summary>
    /// 对象容器
    /// </summary>
    public Queue<GameObject> poolContainer;

    private int maxCount = int.MaxValue;
    public int MaxCount
    {
        get { return maxCount; }
        set
        {
            maxCount = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }

    public PoolData(GameObject obj, GameObject poolObj)
    {
        parent = new GameObject(obj.name + "Pool");
        parent.transform.parent = poolObj.transform;
        poolContainer = new Queue<GameObject>();
    }
    public PoolData(GameObject obj, GameObject poolObj, int max)
    {
        parent = new GameObject(obj.name + "Pool");
        parent.transform.parent = poolObj.transform;
        poolContainer = new Queue<GameObject>();
        MaxCount = max;
    }
}
