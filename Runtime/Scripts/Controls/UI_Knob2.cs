//Credit BlueSummerApp.com
/// Sourced from - https://github.com/Unity-UI-Extensions/com.unity.uiextensions/discussions/416#discussioncomment-11534121

using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// Alternate UI Knob control that allows for multiple full rotations to set a value within a specified range.
    /// </summary>
    /// <remarks>
    /// You have a new object for the knob handle, so the background picture doesn`t rotate with the handle. As handle you can use a transparent png file with a round red dot or something alike. Kind regards, Jens. If you have questions you can write to info@bluesummerapp.com.
    /// </remarks>
    public class UI_Knob2 : MonoBehaviour, IDragHandler, IInitializePotentialDragHandler
    {
        public float CurrentValue { get; private set; }

        [Header("Knob Settings")]
        [Min(0f)]
        public float MinValue = 0f; // Minimum value of the knob
        [Min(0f)]
        public float MaxValue = 100f; // Maximum value of the knob
        [Min(0f)]
        public float MaxRotation = 720f; // Maximum rotation of the knob (e.g., 720 for 2 full loops)

        [Header("UI References")]
        public RectTransform KnobHandle; // Knob's RectTransform

        public UnityEvent<float> ValueChanged => _valueChanged;
        [SerializeField] private UnityEvent<float> _valueChanged = new UnityEvent<float>();

        private float _currentAngle = 0f; // Current rotation angle
        private Vector2 _knobCenter;

        private float _startDragAngle = 0f;
        private float _accumulatedRotation = 0f;

        void Start()
        {
            if (KnobHandle == null)
            {
                Debug.LogError("Knob Handle is not assigned.");
            }

            // Calculate the center of the knob in local space
            RectTransform rectTransform = GetComponent<RectTransform>();
            _knobCenter = rectTransform.rect.center;
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                KnobHandle.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localMousePosition
            );

            // Calculate the angle relative to the center of the knob
            _startDragAngle = Mathf.Atan2(localMousePosition.y - _knobCenter.y, localMousePosition.x - _knobCenter.x) * Mathf.Rad2Deg;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                KnobHandle.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localMousePosition
            );

            // Calculate the angle relative to the center of the knob
            float currentDragAngle = Mathf.Atan2(localMousePosition.y - _knobCenter.y, localMousePosition.x - _knobCenter.x) * Mathf.Rad2Deg;

            // Calculate the delta angle
            float deltaAngle = Mathf.DeltaAngle(currentDragAngle, _startDragAngle);
            _startDragAngle = currentDragAngle;

            // Update the accumulated rotation and clamp it
            _accumulatedRotation = Mathf.Clamp(_accumulatedRotation + deltaAngle, 0f, MaxRotation);

            // Rotate the knob
            _currentAngle = _accumulatedRotation % 360f;
            KnobHandle.localEulerAngles = new Vector3(0, 0, -_currentAngle);

            // Interpolate the value based on the rotation
            CurrentValue = Mathf.Lerp(MinValue, MaxValue, _accumulatedRotation / MaxRotation);
            _valueChanged.Invoke(CurrentValue);
        }

        public void SetValueWithoutNotify(float value)
        {
            // Clamp the value within the range
            CurrentValue = Mathf.Clamp(value, MinValue, MaxValue);

            // Calculate the corresponding rotation based on the value
            _accumulatedRotation = Mathf.Lerp(0f, MaxRotation, (CurrentValue - MinValue) / (MaxValue - MinValue));

            // Update the knob's rotation
            _currentAngle = _accumulatedRotation % 360f;
            KnobHandle.localEulerAngles = new Vector3(0, 0, -_currentAngle);
        }
    }
}