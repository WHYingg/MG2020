using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共MonoBehaviour模块
/// 提供给外部添加帧更新事件的方法
/// 提供给外部添加携程的方法
/// </summary>
public class MonoManager : ManagerBase<MonoManager>
{
    private MonoController controller;

    #region Updata

    /// <summary>
    /// 给外部提供帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddUpdateListener(UnityAction fun)
    {
        controller.AddUpdateListener(fun);
    }

    /// <summary>
    /// 移除外部提供帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveUpdateListener(UnityAction fun)
    {
        controller.RemoveUpdateListener(fun);
    }

    /// <summary>
    /// 给外部提供固定帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void AddFixUpdateListener(UnityAction fun)
    {
        controller.AddFixUpdateListener(fun);
    }

    /// <summary>
    /// 提供移除外部固定帧更新事件函数
    /// </summary>
    /// <param name="fun"></param>
    public void RemoveFixUpdateListener(UnityAction fun)
    {
        controller.RemoveFixUpdateListener(fun);
    }

    #endregion

    #region 携程

    public Coroutine StartCoroutine(string methodName)
    {
        return controller.StartCoroutine(methodName);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(methodName, value);
    }

    public void StopAllCoroutines()
    {
        controller.StopAllCoroutines();
    }

    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }

    public void StopCoroutine(Coroutine routine)
    {
        controller.StopCoroutine(routine);
    }

    public void StopCoroutine(string methodName)
    {
        controller.StopCoroutine(methodName);
    }

    #endregion

    public MonoManager()
    {
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }
}
