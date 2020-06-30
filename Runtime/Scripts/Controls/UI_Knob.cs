/// Credit Tomasz Schelenz 
/// Sourced from - https://bitbucket.org/ddreaper/unity-ui-extensions/issues/46/feature-uiknob#comment-29243988

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
        public float knobValue;
        [Tooltip("Max value of the knob, maximum RAW output value knob can reach, overrides snap step, IF set to 0 or higher than loops, max value will be set by loops")]
        public float maxValue = 0;
        [Tooltip("How many rotations knob can do, if higher than max value, the latter will limit max value")]
        public int loops = 1;
        [Tooltip("Clamp output value between 0 and 1, useful with loops > 1")]
        public bool clampOutput01 = false;
        [Tooltip("snap to position?")]
        public bool snapToPosition = false;
        [Tooltip("Number of positions to snap")]
        public int snapStepsPerLoop = 10;
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
                knobValue = 1 - (finalRotation.eulerAngles.z / 360f);

                if (snapToPosition)
                {
                    SnapToPosition(ref knobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 - 360 * knobValue);
                }
            }
            else
            {
                knobValue = (finalRotation.eulerAngles.z / 360f);

                if (snapToPosition)
                {
                    SnapToPosition(ref knobValue);
                    finalRotation.eulerAngles = new Vector3(0, 0, 360 * knobValue);
                }
            }

            //PREVENT OVERROTATION
            if (Mathf.Abs(knobValue - _previousValue) > 0.5f)
            {
                if (knobValue < 0.5f && loops > 1 && _currentLoops < loops - 1)
                {
                    _currentLoops++;
                }
                else if (knobValue > 0.5f && _currentLoops >= 1)
                {
                    _currentLoops--;
                }
                else
                {
                    if (knobValue > 0.5f && _currentLoops == 0)
                    {
                        knobValue = 0;
                        transform.localEulerAngles = Vector3.zero;
                        InvokeEvents(knobValue + _currentLoops);
                        return;
                    }
                    else if (knobValue < 0.5f && _currentLoops == loops - 1)
                    {
                        knobValue = 1;
                        transform.localEulerAngles = Vector3.zero;
                        InvokeEvents(knobValue + _currentLoops);
                        return;
                    }
                }
            }

            //CHECK MAX VALUE
            if (maxValue > 0)
            {
                if (knobValue + _currentLoops > maxValue)
                {
                    knobValue = maxValue;
                    float maxAngle = direction == Direction.CW ? 360f - 360f * maxValue : 360f * maxValue;
                    transform.localEulerAngles = new Vector3(0, 0, maxAngle);
                    InvokeEvents(knobValue);
                    return;
                }
            }

            transform.rotation = finalRotation;
            InvokeEvents(knobValue + _currentLoops);

            _previousValue = knobValue;
        }
        private void SnapToPosition(ref float knobValue)
        {
            float snapStep = 1 / (float)snapStepsPerLoop;
            float newValue = Mathf.Round(knobValue / snapStep) * snapStep;
            knobValue = newValue;
        }
        private void InvokeEvents(float value)
        {
            if (clampOutput01)
                value /= loops;
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