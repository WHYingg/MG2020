using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mono单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject(typeof(T).ToString());
            instance = obj.AddComponent<T>();
            DontDestroyOnLoad(obj);
        }
        return instance;
    }
}
