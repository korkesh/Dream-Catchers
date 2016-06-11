using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ControllerFocus : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(EventSystem.current.currentSelectedGameObject == null)
         {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }
}
