using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class SceneSwitchManager : ManagerBase<SceneSwitchManager>
{
    #region 同步加载场景

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="fun">加载场景后执行的方法</param>
    public void LoadScene(string name, UnityAction fun = null)
    {
        SceneManager.LoadScene(name);
        fun?.Invoke();
        EventCenter.Broadcast(EventDefine.SceneSwitch);
    }

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="sceneBuildIndex">场景序号</param>
    /// <param name="fun">场景加载完成后执行的方法</param>
    public void LoadScene(int sceneBuildIndex, UnityAction fun = null)
    {
        SceneManager.LoadScene(sceneBuildIndex);
        fun?.Invoke();
        EventCenter.Broadcast(EventDefine.SceneSwitch);
    }

    /// <summary>
    /// 延迟同步加载场景
    /// </summary>
    /// <param name="sceneBuildIndex">场景序号</param>
    /// <param name="fun">场景加载完成后执行的方法</param>
    public void LoadScene(int sceneBuildIndex, float t, UnityAction fun = null)
    {
        MonoManager.GetInstance().StartCoroutine(WaitLoad(sceneBuildIndex, t, fun));

    }

    IEnumerator WaitLoad(int sceneBuildIndex, float t, UnityAction fun = null)
    {
        yield return new WaitForSeconds(t);
        SceneManager.LoadScene(sceneBuildIndex);
        fun?.Invoke();
        EventCenter.Broadcast(EventDefine.SceneSwitch);
    }

    #endregion

    #region 异步加载场景

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="isForSlider">是否由滑动条显示加载</param>
    /// <param name="fun">场景加载完成后需要调用的方法</param>
    public void LoadSceneAsync(string name, bool isForSlider = false, UnityAction fun = null)
    {
        MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsync(name, isForSlider, fun));
    }

    private IEnumerator ReallyLoadSceneAsync(string name, bool isForSlider = false, UnityAction fun = null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        if (isForSlider)
        {
            ao.allowSceneActivation = false;
        }
        while (!ao.isDone)
        {
            EventCenter.Broadcast(EventDefine.LoadSceneAsyn, ao);
            yield return ao.progress;
        }
        if (!isForSlider)
        {
            EventCenter.Broadcast(EventDefine.SceneSwitch);
        }
        fun?.Invoke();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneBuildIndex">场景序号</param>
    /// <param name="isForSlider">是否由滑动条显示加载</param>
    /// <param name="fun">场景加载完成后需要调用的方法</param>
    public void LoadSceneAsync(int sceneBuildIndex, bool isForSlider = false, UnityAction fun = null)
    {
        MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(sceneBuildIndex, isForSlider, fun));
    }

    private IEnumerator ReallyLoadSceneAsyn(int sceneBuildIndex, bool isForSlider, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex);
        if (isForSlider)
        {
            ao.allowSceneActivation = false;
        }
        while (!ao.isDone)
        {
            EventCenter.Broadcast(EventDefine.LoadSceneAsyn, ao);
            yield return ao.progress;
        }
        if (!isForSlider)
        {
            EventCenter.Broadcast(EventDefine.SceneSwitch);
        }
        fun?.Invoke();
    }

    #endregion

}
