using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>
//[CreateAssetMenu(menuName = "ResourceManagerContainer")]
public class ResourceManager : ScriptableObject
{
    private static ResourceManager instance;

    public GameObject TestObj;

    #region 同步加载资源

    public T load<T>(string name)where T:Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        else
        {
            return res;
        }
    }

    #endregion

    #region 异步加载
    public void LoadAsync<T>(string name,UnityAction<T> callback) where T : Object
    {
        MonoManager.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }

    public IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;
        if (r.asset is GameObject)
        {
            callback(GameObject.Instantiate(r.asset) as T);

        }
        else
        {
            callback(r.asset as T);
        }
    }

    #endregion

    public static ResourceManager GetResourceManager()
    {
        if (instance == null)
        {
            instance = Resources.Load<ResourceManager>("ResourceManagerContainer");
        }
        return instance;
    }
}
