/// Credit SimonDarksideJ

using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Handy Selectable script to un-highlight a selectable component in Unity (e.g. a Button) when the user moves away from it, EVEN IF the user has holding a button on it.
    /// Resolves the situation where Unity UI Components remain in a highlighted state even after the pointer has moved away (e.g. user holding a button, mouse, pointer down).
    /// Now whenever the cursor leaves the component, it will force the UI component to revert to un-highlighted.
    /// </summary>
    [AddComponentMenu("UI/Extensions/ResetSelectableHighlight", 31)]
    [RequireComponent(typeof(Selectable))]
    public class ResetSelectableHighlight : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField]
        private Selectable attachedSelectable = null;

        private void Awake()
        {
            if (attachedSelectable == null || !attachedSelectable)
            {
                attachedSelectable = GetComponent<Selectable>();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!attachedSelectable.interactable)
            {
                return;
            }

            attachedSelectable.targetGraphic.CrossFadeColor(attachedSelectable.colors.normalColor, 0f, true, true);
        }
    }
}
