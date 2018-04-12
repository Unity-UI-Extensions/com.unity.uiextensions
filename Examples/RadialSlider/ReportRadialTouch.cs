using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReportRadialTouch : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public Text OutputField;

    #region Interfaces
    // Called when the pointer enters our GUI component.
    // Start tracking the mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        OutputField.text = "Enter - eligibleForClick [" + eventData.eligibleForClick.ToString() + "] - pointerId [ " + eventData.pointerId + "]";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OutputField.text = "Pointer Down - eligibleForClick [" + eventData.eligibleForClick.ToString() + "] - pointerId [ " + eventData.pointerId + "]";
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OutputField.text = "Pointer Up - eligibleForClick [" + eventData.eligibleForClick.ToString() + "] - pointerId [ " + eventData.pointerId + "]";
    }
    #endregion
}
