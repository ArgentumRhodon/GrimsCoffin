using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStateMachine : MonoBehaviour
{
    public string customName;

    private CState mainStateType;

    public CState CurrentState { get; private set; }
    private CState nextState;
    public bool RegisteredAttack { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (nextState != null)
        {
            SetState(nextState);
        }

        //Debug.Log(CurrentState);

        if (CurrentState != null)
            CurrentState.OnUpdate(this);
    }

    private void SetState(CState _newState)
    {
        nextState = null;
        if (CurrentState != null)
        {
            CurrentState.OnExit();
        }
        CurrentState = _newState;
        CurrentState.OnEnter(this);
    }

    public void SetNextState(CState _newState)
    {
        if (_newState != null)
        {
            nextState = _newState;
        }
    }

    private void LateUpdate()
    {
        if (CurrentState != null)
            CurrentState.OnLateUpdate();
    }

    private void FixedUpdate()
    {
        if (CurrentState != null)
            CurrentState.OnFixedUpdate();
    }

    public void SetNextStateToMain()
    {
        nextState = mainStateType;
    }

    private void Start()
    {
        SetNextStateToMain();
    }


    private void Awake()
    {
        if (mainStateType == null)
        {
            if (customName == "Combat")
            {
                mainStateType = new IdleCombatState();
            }
        }
    }
}
