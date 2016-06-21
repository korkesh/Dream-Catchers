//================================
// Alex
//  base for FSM
//================================
using UnityEngine;
using System.Collections;

public abstract class BaseState : MonoBehaviour
{

    //================================
    // Variables
    //================================
    public string Statename;
    public FSM fsm;


    //================================
    // Abstract Methods
    //================================
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();


}
