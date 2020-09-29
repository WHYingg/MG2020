using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Transition
{
    NullTransition = 0,
}
public enum StateID
{
    NullStateID = 0,
}

/// <summary>
/// 状态基类
/// </summary>
public abstract class FSMState
{

    protected StateID stateID;
    public StateID ID { get { return stateID; } }
    protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();//条件->状态
    protected FSMSystem fsm;

    public FSMState(FSMSystem fsm,StateID stateID)
    {
        this.fsm = fsm;
        this.stateID = stateID;
    }

    /// <summary>
    /// 添加转换条件
    /// </summary>
    /// <param name="trans">转换条件</param>
    /// <param name="id">状态</param>
    public void AddTransition(Transition trans, StateID id)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("不允许NullTransition"); return;
        }
        if (id == StateID.NullStateID)
        {
            Debug.LogError("不允许NullStateID"); return;
        }
        if (map.ContainsKey(trans))
        {
            Debug.LogError("添加转换条件的时候，" + trans + "已经存在于map中"); return;
        }
        map.Add(trans, id);
    }

    /// <summary>
    /// 删除转换条件
    /// </summary>
    /// <param name="trans">转换条件</param>
    public void DeleteTransition(Transition trans)
    {
        if (trans == Transition.NullTransition)
        {
            Debug.LogError("不允许NullTransition"); return;
        }
        if (map.ContainsKey(trans) == false)
        {
            Debug.LogError("删除转换条件的时候，" + trans + "不存在于map中"); return;
        }
        map.Remove(trans);
    }

    /// <summary>
    /// 得到转换条件对应的状态
    /// </summary>
    /// <param name="trans">转换条件</param>
    /// <returns></returns>
    public StateID GetOutputState(Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.NullStateID;
    }

    /// <summary>
    /// 进入状态前
    /// </summary>
    public virtual void DoBeforeEntering() { }
    /// <summary>
    /// 离开状态时
    /// </summary>
    public virtual void DoAfterLeaving() { }
    /// <summary>
    /// 该状态下npc对应的行为
    /// </summary>
    public abstract void Action(GameObject npc);
    /// <summary>
    /// 判断转换条件
    /// </summary>
    public abstract void Reason(GameObject npc);
}
