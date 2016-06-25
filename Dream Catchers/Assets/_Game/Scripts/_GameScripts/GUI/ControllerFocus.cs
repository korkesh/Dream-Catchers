///=====================================================================================
/// Author: Matt
/// Purpose: Sets menu focus to controller if game focus is lost (i.e. an alt-tab)
///======================================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ControllerFocus : MonoBehaviour {

	// Update is called once per frame
	void Update () {

        if(EventSystem.current.currentSelectedGameObject == null)
         {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }
}
