using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CheckPlace
{
    OnUpdate,
    OnFixUpdate
}

/// <summary>
/// 输入控制模块
/// </summary>
public class InputManager : ManagerBase<InputManager>
{
    private bool isStart = false;

    private void OnUpdate()
    {
        if (isStart == false)
        {
            return;
        }
        CheckKeyCod(KeyCode.W,CheckPlace.OnUpdate);
        CheckKeyCod(KeyCode.A, CheckPlace.OnUpdate);
        CheckKeyCod(KeyCode.S, CheckPlace.OnUpdate);
        CheckKeyCod(KeyCode.D, CheckPlace.OnUpdate);
        CheckKeyCod(KeyCode.Space, CheckPlace.OnUpdate);
    }

    private void OnFixUpdate()
    {
        if (isStart == false)
        {
            return;
        }
        CheckKeyCod(KeyCode.W,CheckPlace.OnFixUpdate);
        CheckKeyCod(KeyCode.A, CheckPlace.OnFixUpdate);
        CheckKeyCod(KeyCode.S, CheckPlace.OnFixUpdate);
        CheckKeyCod(KeyCode.D, CheckPlace.OnFixUpdate);
        CheckKeyCod(KeyCode.Space, CheckPlace.OnFixUpdate);
    }

    /// <summary>
    /// 是否开启或关闭输入检测
    /// </summary>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }

    /// <summary>
    /// 广播按键的按下或放开
    /// </summary>
    private void CheckKeyCod(KeyCode key, CheckPlace place)
    {
        if (Input.GetKeyDown(key))
        {
            EventCenter.Broadcast(EventDefine.KeyDown, key,place);
        }
        if (Input.GetKeyUp(key))
        {
            EventCenter.Broadcast(EventDefine.KeyUp, key,place);
        }
    }

    public InputManager()
    {
        MonoManager.GetInstance().AddUpdateListener(OnUpdate);
        MonoManager.GetInstance().AddFixUpdateListener(OnFixUpdate);
    }
}
