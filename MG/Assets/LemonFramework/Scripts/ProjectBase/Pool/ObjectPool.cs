using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 对象池
/// </summary>
public class ObjectPool :SingletonMono<ObjectPool>
{
    //所有池的存储
    private Dictionary<string, PoolData> pools;

    //初始化
    void Awake()
    {
        pools = new Dictionary<string, PoolData>();

    }

    #region 初始化对象池

    /// <summary>
    /// 池子创建与物体预热/预加载
    /// </summary>
    /// <param name="go">需要预热的物体</param>
    /// <param name="number">需要预热的数量</param>
    /// TODO 既然有预热用空间换时间 应该要做一个清理用时间换空间的功能
    public void Preload(GameObject go, int number)
    {
        if (!pools.ContainsKey(go.name))
        {
            pools.Add(go.name, new PoolData(go, gameObject));

        }
        else
        {
            Debug.LogWarning(go.name + "对象池现才创建,建议创建对象池时一次性预热所有物体。");
        }
        for (int i = 0; i < number; i++)
        {
            GameObject newObject = Instantiate(go, pools[go.name].parent.transform);
            newObject.name = go.name;//确认名字一样，防止系统加一个(clone),或序号累加之类的
            newObject.SetActive(false);
            pools[go.name].poolContainer.Enqueue(newObject);
        }
    }

    /// <summary>
    /// 池子创建与物体预热/预加载
    /// </summary>
    /// <param name="go">需要预热的物体</param>
    /// <param name="number">需要预热的数量</param>
    /// <param name="number">池子的最大数量</param>
    public void Preload(GameObject go, int number, int maxCount)
    {
        if (!pools.ContainsKey(go.name))
        {
            pools.Add(go.name, new PoolData(go, gameObject, maxCount));
        }
        else
        {
            Debug.LogWarning(go.name + "对象池现才创建,建议创建对象池时一次性预热所有物体。");
        }
        for (int i = 0; i < number; i++)
        {
            GameObject newObject = Instantiate(go, pools[go.name].parent.transform);
            newObject.name = go.name;//确认名字一样，防止系统加一个(clone),或序号累加之类的
            newObject.SetActive(false);
            pools[go.name].poolContainer.Enqueue(newObject);
        }
    }

    #endregion

    #region 从池中获取物体

    /// <summary>
    /// 从池中获取物体
    /// </summary>
    /// <param name="go">需要取得的物体</param>
    /// <param name="position">位置</param>
    /// <param name="rotation">旋转</param>
    /// <returns></returns>
    public GameObject GetObject(GameObject go, Vector3 position, Quaternion rotation)
    {
        //如果未初始化过 初始化池
        if (!pools.ContainsKey(go.name))
        {
            pools.Add(go.name, new PoolData(go, gameObject));
            Debug.LogWarning("未进行" + go.name + "对象池的预创建与物体预加载，现已创建" + go.name + "对象池。");
        }
        //如果池空了就创建新物体
        if (pools[go.name].poolContainer.Count == 0)
        {
            GameObject newObject = Instantiate(go, position, rotation);
            newObject.name = go.name;
            newObject.transform.parent = pools[go.name].parent.transform;
            Debug.LogWarning(go.name + "对象池已空，现直接调用Instantiate方法获取GameObject。");
            /*
            确认名字一样，防止系统加一个(clone),或序号累加之类的
            实际上为了更健全可以给每一个物体加一个key，防止对象的name一样但实际上不同
             */
            return newObject;
        }
        //从池中获取物体
        GameObject nextObject = pools[go.name].poolContainer.Dequeue();
        //  nextObject.SetActive(true);//要先启动再设置属性，否则可能会被OnEnable重置
        if (nextObject != null)//先设置属性再启动，避免玩家看到位移过程
        {
            nextObject.transform.position = position;
            nextObject.transform.rotation = rotation;
            nextObject.SetActive(true);
        }
        return nextObject;
    }

    public GameObject GetObject(GameObject go, Vector3 position, Quaternion rotation, Transform parent)
    {
        //如果未初始化过 初始化池
        if (!pools.ContainsKey(go.name))
        {
            pools.Add(go.name, new PoolData(go, gameObject));
        }
        //如果池空了就创建新物体
        if (pools[go.name].poolContainer.Count == 0)
        {
            GameObject newObject = Instantiate(go, position, rotation,parent);
            newObject.name = go.name;
            newObject.transform.parent = parent;
            Debug.LogWarning(go.name + "对象池已空，现直接调用Instantiate方法获取GameObject。");/*
            确认名字一样，防止系统加一个(clone),或序号累加之类的
            实际上为了更健全可以给每一个物体加一个key，防止对象的name一样但实际上不同
             */
            return newObject;
        }
        //从池中获取物体
        GameObject nextObject = pools[go.name].poolContainer.Dequeue();
        //  nextObject.SetActive(true);//要先启动再设置属性，否则可能会被OnEnable重置
        nextObject.transform.position = position;
        nextObject.transform.rotation = rotation;
        nextObject.SetActive(true);
        nextObject.transform.parent = parent;
        return nextObject;
    }

    #endregion

    #region 把物体放回池里

    /// <summary>
    /// 把物体放回池里
    /// </summary>
    /// <param name="go">需要放回池子的对象</param>
    /// <param name="t">延迟执行的时间</param>
    /// TODO 应该做个检查put的gameobject的池有没有创建过池
    public void PutObject(GameObject go, float t)
    {
        if (pools.ContainsKey(go.name))
        {
            if (pools[go.name].poolContainer.Count >= pools[go.name].MaxCount)
                Destroy(go, t);
            else
                StartCoroutine(ExecutePut(go, t));
        }
        else
        {
            Debug.LogError(go.name + "对象池不存在。");
        }

    }

    private IEnumerator ExecutePut(GameObject go, float t)
    {
        yield return new WaitForSeconds(t);
        if (go != null)
        {
            go.SetActive(false);
            pools[go.name].poolContainer.Enqueue(go);
        }

    }

    public void PutObject(GameObject go)
    {
        if (pools.ContainsKey(go.name))
        {
            if (pools[go.name].poolContainer.Count >= pools[go.name].MaxCount)
                Destroy(go);
            else if (go != null)
            {
                go.SetActive(false);
                pools[go.name].poolContainer.Enqueue(go);
            }
        }
        else
        {
            Debug.LogError(go.name + "对象池不存在。");
        }

    }

    #endregion

    #region 清空对象池

    private void Clear()
    {
        pools.Clear();
    }

    private void OnDestroy()
    {
        Clear();
    }

    #endregion

}
