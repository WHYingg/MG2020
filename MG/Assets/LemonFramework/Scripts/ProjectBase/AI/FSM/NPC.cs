using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 示例脚本
/// </summary>
public class NPC : MonoBehaviour
{
    private FSMSystem fsm;

    void Start()
    {
        fsm = new FSMSystem();

        //FSMState state = new AanayState(fsm,StateID.NullStateID);
        //state.AddTransition(Transition.NullTransition, StateID.NullStateID);
        //fsm.AddState(state);
    }

    void Update()
    {
        fsm.OnUpdate(gameObject);
    }
}
