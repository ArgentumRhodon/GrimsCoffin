using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CState
{
    protected float time { get; set; }
    protected float fixedtime { get; set; }
    protected float latetime { get; set; }

    public CStateMachine stateMachine;

    public virtual void OnEnter(CStateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
    }


    public virtual void OnUpdate(CStateMachine _stateMachine)
    {
        time += Time.deltaTime;
    }

    public virtual void OnFixedUpdate()
    {
        fixedtime += Time.deltaTime;
    }
    public virtual void OnLateUpdate()
    {
        latetime += Time.deltaTime;
    }

    public virtual void OnExit()
    {

    }
}
