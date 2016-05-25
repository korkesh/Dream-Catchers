using UnityEngine;
using System.Collections;

public abstract class BaseState : MonoBehaviour
{

    public string Statename;
    public FSM fsm;



    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();


}
