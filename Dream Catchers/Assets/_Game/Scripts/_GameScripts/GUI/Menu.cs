//================================
// Alex
//  menu base class for menu animation stuff
//================================
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    //================================
    // Variables
    //================================

    private Animator _animator;
    private CanvasGroup _canvasGroup;
    public Selectable firstSelected;
    public Selectable secondSelected;

     //================================
     // Methods
     //================================


     //-----------------
     // Initialization
     //-----------------

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        _canvasGroup = GetComponent<CanvasGroup>();

    }

    //-----------------
    // Update
    //-----------------
    public void Update()
    {

        //if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        //{
        //    _canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
        //}
        //else
        //{
        //    _canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
        //}
    }


    /// <summary>
    /// changes animation bool so that the menu is now the shown menu
    /// </summary>
    public bool IsOpen
    {
        get { return _animator.GetBool("IsOpen"); }
        set { _animator.SetBool("IsOpen", value); }
    }
}
