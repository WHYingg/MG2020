using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;

    private event UnityAction fixUpdateEvent;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        updateEvent?.Invoke();
    }

    void FixedUpdate()
    {
        fixUpdateEvent?.Invoke();
    }

    /// <summary>
    /// 给外部提供帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    /// <summary>
    /// 提供移除外部帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }

    /// <summary>
    /// 给外部提供固定帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddFixUpdateListener(UnityAction fun)
    {
        fixUpdateEvent += fun;
    }

    /// <summary>
    /// 提供移除外部固定帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveFixUpdateListener(UnityAction fun)
    {
        fixUpdateEvent -= fun;
    }
}
