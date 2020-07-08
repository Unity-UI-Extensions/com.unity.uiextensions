/// Credit Chris Trueman
/// Sourced from - http://forum.unity3d.com/threads/use-reticle-like-mouse-for-worldspace-uis.295271/

namespace UnityEngine.EventSystems.Extensions
{
    [RequireComponent(typeof(EventSystem))]
    [AddComponentMenu("Event/Extensions/Aimer Input Module")]
    public class AimerInputModule : PointerInputModule
    {
        /// <summary>
        /// The Input axis name used to activate the object under the reticle.
        /// </summary>
        public string activateAxis = "Submit";

        /// <summary>
        /// The aimer offset position. Aimer is center screen use this offset to change that.
        /// </summary>
        public Vector2 aimerOffset = new Vector2(0, 0);

        /// <summary>
        /// The object under aimer. A static access field that lets you know what is under the aimer.
        /// This field can return null.
        /// </summary>
        public static GameObject objectUnderAimer;

        protected AimerInputModule() { }

        public override void ActivateModule()
        {
            StandaloneInputModule StandAloneSystem = GetComponent<StandaloneInputModule>();

            if (StandAloneSystem != null && StandAloneSystem.enabled)
            {
                Debug.LogError("Aimer Input Module is incompatible with the StandAloneInputSystem, " +
                    "please remove it from the Event System in this scene or disable it when this module is in use");
            }
        }

        public override void Process()
        {
            bool pressed = Input.GetButtonDown(activateAxis);
            bool released = Input.GetButtonUp(activateAxis);

            PointerEventData pointer = GetAimerPointerEventData();

            ProcessInteraction(pointer, pressed, released);

            if (!released)
                ProcessMove(pointer);
            else
                RemovePointerData(pointer);
        }

        protected virtual PointerEventData GetAimerPointerEventData()
        {
            PointerEventData pointerData;

            //Not certain on the use of this.
            //I know that -1 is the mouse and anything positive would be a finger/touch, 0 being the first finger, 1 being the second and so one till the system limit is reached.
            //So that is the reason I choose -2.
            GetPointerData(-2, out pointerData, true);

            pointerData.Reset();

            pointerData.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f) + aimerOffset;

            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();
            return pointerData;
        }

        private void ProcessInteraction(PointerEventData pointer, bool pressed, bool released)
        {
            var currentOverGo = pointer.pointerCurrentRaycast.gameObject;

            objectUnderAimer = ExecuteEvents.GetEventHandler<ISubmitHandler>(currentOverGo);//we only want objects that we can submit on.

            if (pressed)
            {
                pointer.eligibleForClick = true;
                pointer.delta = Vector2.zero;
                pointer.pressPosition = pointer.position;
                pointer.pointerPressRaycast = pointer.pointerCurrentRaycast;

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointer, ExecuteEvents.submitHandler);

                // didn't find a press handler... search for a click handler
                if (newPressed == null)
                {
                    newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointer, ExecuteEvents.pointerDownHandler);
                    if (newPressed == null)
                        newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
                }
                else
                {
                    pointer.eligibleForClick = false;
                }

                if (newPressed != pointer.pointerPress)
                {
                    pointer.pointerPress = newPressed;
                    pointer.rawPointerPress = currentOverGo;
                    pointer.clickCount = 0;
                }

                // Save the drag handler as well
                pointer.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointer.pointerDrag != null)
                    ExecuteEvents.Execute<IBeginDragHandler>(pointer.pointerDrag, pointer, ExecuteEvents.beginDragHandler);
            }

            if (released)
            {
                //Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointer.pointerPress, pointer, ExecuteEvents.pointerUpHandler);

                //Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick
                if (pointer.pointerPress == pointerUpHandler && pointer.eligibleForClick)
                {
                    float time = Time.unscaledTime;

                    if (time - pointer.clickTime < 0.3f)
                        ++pointer.clickCount;
                    else
                        pointer.clickCount = 1;
                    pointer.clickTime = time;

                    ExecuteEvents.Execute(pointer.pointerPress, pointer, ExecuteEvents.pointerClickHandler);
                }
                else if (pointer.pointerDrag != null)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointer, ExecuteEvents.dropHandler);
                }

                pointer.eligibleForClick = false;
                pointer.pointerPress = null;
                pointer.rawPointerPress = null;

                if (pointer.pointerDrag != null)
                    ExecuteEvents.Execute(pointer.pointerDrag, pointer, ExecuteEvents.endDragHandler);

                pointer.pointerDrag = null;
            }
        }

        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }
    }
}