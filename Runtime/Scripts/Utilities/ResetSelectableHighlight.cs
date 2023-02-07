/// Credit SimonDarksideJ

namespace UnityEngine.UI.Extensions
{
    /*
    Handy Selectable script to un-highlight a selectable component in Unity (e.g. a Button) when the user moves away from it, EVEN IF the user has holding a button on it.

    Resolves the situation where Unity UI Components remain in a highlighted state even after the pointer has moved away (e.g. user holding a button, mouse, pointer down).
    Now whenever the cursor leaves the component, it will force the UI component to revert to un-highlighted.
    */

    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [AddComponentMenu("UI/Extensions/ResetSelectableHighlight", 31)]
    [RequireComponent(typeof(Selectable))]
    public class ResetSelectableHighlight : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField]
        private Selectable attachedSelectable;

        // Start is called before the first frame update
        void Awake()
        {
            if (!attachedSelectable)
            {
                attachedSelectable = GetComponent<Selectable>();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            attachedSelectable.targetGraphic.CrossFadeColor(attachedSelectable.colors.normalColor, 0f, true, true);
        }
    }
}