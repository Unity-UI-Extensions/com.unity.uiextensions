/// Credit AriathTheWise
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1796783

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
 
 
/// <summary>
/// UIButton
/// </summary>
public class UIButton : Button, IPointerDownHandler, IPointerUpHandler
{
    #region Sub-Classes
    [System.Serializable]
    public class UIButtonEvent : UnityEvent<PointerEventData.InputButton> { }
    #endregion
 
 
    #region Events
    public UIButtonEvent OnButtonClick;
    public UIButtonEvent OnButtonPress;
    public UIButtonEvent OnButtonRelease;
    #endregion
   
 
 
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnSubmit(eventData);
 
        if (OnButtonClick != null)
        {
            OnButtonClick.Invoke(eventData.button);
        }
    }
   
 
    void IPointerDownHandler.OnPointerDown (PointerEventData eventData)
    {
        DoStateTransition(SelectionState.Pressed, false);
 
        if (OnButtonPress != null)
        {
            OnButtonPress.Invoke(eventData.button);
        }
    }
 
 
    void IPointerUpHandler.OnPointerUp (PointerEventData eventData)
    {
        DoStateTransition(SelectionState.Normal, false);        
 
        if (OnButtonRelease != null)
        {
            OnButtonRelease.Invoke(eventData.button);
        }
    }    
}