/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/SimonDarksideJ/unity-ui-extensions/issues/46/feature-uiknob#comment-29243988

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// KNOB controller
/// 
/// Fields
/// - direction - direction of rotation CW - clockwise CCW - counter clock wise
/// - knobValue - Output value of the control
/// - maxValue - max value knob can rotate to, if higher than loops value or set to 0 - it will be ignored, and max value will be based on loops
/// - loops - how any turns around knob can do
/// - clampOutput01 - if true the output knobValue will be clamped between 0 and 1 regardless of number of loops.
/// - snapToPosition - snap to step. NOTE: max value will override the step.
/// - snapStepsPerLoop - how many snap positions are in one knob loop;
/// - OnValueChanged - event that is called every frame while rotating knob, sends <float> argument of knobValue
/// NOTES
/// - script works only in images rotation on Z axis;
/// - while dragging outside of control, the rotation will be canceled
/// </summary>
/// 
namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Extensions/UI_Knob")]
    public class UI_Knob : Selectable, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IInitializePotentialDragHandler
    {
        public enum Direction { CW, CCW };
        [Tooltip("Direction of rotation CW - clockwise, CCW - counterClockwise")]
        public Direction direction = Direction.CW;
        [HideInInspector]
        public float KnobValue;
        [Tooltip("Max value of the knob, maximum RAW output value knob can reach, overrides snap step, IF set to 0 or higher than loops, max value will be set by loops")]
        public float MaxValue = 0;
        [Tooltip("How many rotations knob can do, if higher than max value, the latter will limit max value")]
        public int Loops = 0;
        [Tooltip("Clamp output value between 0 and 1, useful with loops > 1")]
        public bool ClampOutput01 = false;
        [Tooltip("snap to position?")]
        public bool SnapToPosition = false;
        [Tooltip("Number of positions to snap")]
        public int SnapStepsPerLoop = 10;
        [Tooltip("Parent touch area to extend the touch radius")]
        public RectTransform ParentTouchMask;
        [Tooltip("Default background color of the touch mask. Defaults as transparent")]
        public Color MaskBackground = new Color(0, 0, 0, 0);
        [Space(30)]
        public KnobFloatValueEvent OnValueChanged;
        private float _currentLoops = 0;
        private float _previousValue = 0;
        private float _initAngle;
        private float _currentAngle;
        private Vector2 _currentVector;
        private Quaternion _initRotation;
        private bool _canDrag = false;
		private bool _screenSpaceOverlay;

        protected override void Awake()
        {
            _screenSpaceOverlay = GetComponentInParent<Canvas>().rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
        }

        protected override void Start()
        {
            CheckForParentTouchMask();
        }

        private void CheckForParentTouchMask()
        {
            if (ParentTouchMask)
            {
                Image maskImage = ParentTouchMask.gameObject.GetOrAddComponent<Image>();
                maskImage.color = MaskBackground;
                EventTrigger trigger = ParentTouchMask.gameObject.GetOrAddComponent<EventTrigger>();
                trigger.triggers.Clear();
                //PointerDownEvent
                EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
                pointerDownEntry.eventID = EventTriggerType.PointerDown;
                pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
                trigger.triggers.Add(pointerDownEntry);
                //PointerUpEvent
                EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
                pointerUpEntry.eventID = EventTriggerType.PointerUp;
                pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
                trigger.triggers.Add(pointerUpEntry);
                //PointerEnterEvent
                EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
                pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
                pointerEnterEntry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
                trigger.triggers.Add(pointerEnterEntry);
                //PointerExitEvent
                EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
                pointerExitEntry.eventID = EventTriggerType.PointerExit;
                pointerExitEntry.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
                trigger.triggers.Add(pointerExitEntry);
                //DragEvent
                EventTrigger.Entry dragEntry = new EventTrigger.Entry();
                dragEntry.eventID = EventTriggerType.Drag;
                dragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
                trigger.triggers.Add(dragEntry);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _canDrag = false;
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            _canDrag = true;
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            _canDrag = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            _canDrag = true;

            base.OnPointerDown(eventData);

            _initRotation = transform.rotation;
			if (_screenSpaceOverlay)
            {
				_currentVector = eventData.position - (Vector2)transform.position;
            }
            else
            {
				_currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(transform.position);
            }
            _initAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //CHECK IF CAN DRAG
            if (!_canDrag)
            {
                return;
            }

			if (_screenSpaceOverlay)
			{
				_currentVector = eventData.position - (Vector2)transform.position;
			}
			else
			{
				_currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(transform.position);
			}
            _currentAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;

            Quaternion addRotation = Quaternion.AngleAxis(_currentAngle - _initAngle, this.transform.forward);
            addRotation.eulerAngles = new Vector3(0, 0, addRotation.eulerAngles.z);

            Quaternion finalRotation = _initRotation * addRotation;

            if (direction == Direction.CW)
            {
                KnobValue = 1 - (finalRotation.eulerAngles.z / 360f);

                if (SnapToPosition)
                {
                    SnapToPositionValue(ref KnobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 - 360 * KnobValue);
                }
            }
            else
            {
                KnobValue = (finalRotation.eulerAngles.z / 360f);

                if (SnapToPosition)
                {
                    SnapToPositionValue(ref KnobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 * KnobValue);
                }
            }

            UpdateKnobValue();

            transform.rotation = finalRotation;
            InvokeEvents(KnobValue + _currentLoops);

            _previousValue = KnobValue;
        }

        private void UpdateKnobValue()
        {
            //PREVENT OVERROTATION
            if (Mathf.Abs(KnobValue - _previousValue) > 0.5f)
            {
                if (KnobValue < 0.5f && Loops > 1 && _currentLoops < Loops - 1)
                {
                    _currentLoops++;
                }
                else if (KnobValue > 0.5f && _currentLoops >= 1)
                {
                    _currentLoops--;
                }
                else
                {
                    if (KnobValue > 0.5f && _currentLoops == 0)
                    {
                        KnobValue = 0;
                        transform.localEulerAngles = Vector3.zero;
                        InvokeEvents(KnobValue + _currentLoops);
                        return;
                    }
                    else if (KnobValue < 0.5f && _currentLoops == Loops - 1)
                    {
                        KnobValue = 1;
                        transform.localEulerAngles = Vector3.zero;
                        InvokeEvents(KnobValue + _currentLoops);
                        return;
                    }
                }
            }

            //CHECK MAX VALUE
            if (MaxValue > 0)
            {
                if (KnobValue + _currentLoops > MaxValue)
                {
                    KnobValue = MaxValue;
                    float maxAngle = direction == Direction.CW ? 360f - 360f * MaxValue : 360f * MaxValue;
                    transform.localEulerAngles = new Vector3(0, 0, maxAngle);
                    InvokeEvents(KnobValue);
                    return;
                }
            }
        }

        public void SetKnobValue(float value, int loops = 0)
        {
            Quaternion newRoation = Quaternion.identity;
            KnobValue = value;
            _currentLoops = loops;

            if (SnapToPosition)
            {
                SnapToPositionValue(ref KnobValue);

            }
            if (direction == Direction.CW)
            {
                newRoation.eulerAngles = new Vector3(0, 0, 360 - 360 * KnobValue);
            }
            else
            {
                newRoation.eulerAngles = new Vector3(0, 0, 360 * KnobValue);
            }

            UpdateKnobValue();

            transform.rotation = newRoation;
            InvokeEvents(KnobValue + _currentLoops);

            _previousValue = KnobValue;
        }

        private void SnapToPositionValue(ref float knobValue)
        {
            float snapStep = 1 / (float)SnapStepsPerLoop;
            float newValue = Mathf.Round(knobValue / snapStep) * snapStep;
            knobValue = newValue;
        }
        private void InvokeEvents(float value)
        {
            if (ClampOutput01)
                value /= Loops;
            OnValueChanged.Invoke(value);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }

    [System.Serializable]
    public class KnobFloatValueEvent : UnityEvent<float> { }

}