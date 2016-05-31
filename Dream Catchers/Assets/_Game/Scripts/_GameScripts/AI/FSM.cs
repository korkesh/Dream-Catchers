using UnityEngine;
using System.Collections;


public class FSM : MonoBehaviour
{

    //current state
    public BaseState _currentState;

    //table to keep states
    public Hashtable states;

    //start state
    public string startStateDream;
    public string startStateNightMare;

    ////last safe state
    //[HideInInspector]
    //public string _lastSafeState;


    // Use this for initialization
    void Start()
    {

        states = new Hashtable();
        BaseState[] comps = this.gameObject.GetComponents<BaseState>();
        foreach (BaseState item in comps)
        {
            states.Add(item.Statename, item);

        }
        
        if(ManipulationManager.instance.currentWorldState == ManipulationManager.WORLD_STATE.DREAM)
        {
            _currentState = (BaseState)states[startStateDream];

        }else
        {
            _currentState = (BaseState)states[startStateNightMare];
        }
        
        _currentState.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        if (Game_Manager.instance == null || !Game_Manager.instance.isPaused())
        {
            _currentState.Execute();
        }
       
    }

    //changes the state
    public void changeState(string stat)
    {

        _currentState.Exit();
        _currentState = (BaseState)states[stat];
        _currentState.Enter();
    }
}
