/// Credit mgear, SimonDarksideJ
/// Sourced from - https://forum.unity3d.com/threads/radial-slider-circle-slider.326392/#post-3143582
/// Updated to include lerping features and programmatic access to angle/value

using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Extensions/Sliders/Radial Slider")]
    public class RadialSlider : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private bool isPointerDown, isPointerReleased, lerpInProgress;
        private Vector2 m_localPos, m_screenPos; 
        private float m_targetAngle, m_lerpTargetAngle, m_startAngle, m_currentLerpTime, m_lerpTime;
        private Camera m_eventCamera;
        private Image m_image;

        [SerializeField]
        [Tooltip("Radial Gradient Start Color")]
        private Color m_startColor = Color.green;
        [SerializeField]
        [Tooltip("Radial Gradient End Color")]
        private Color m_endColor = Color.red;
        [Tooltip("Move slider absolute or use Lerping?\nDragging only supported with absolute")]
        [SerializeField]
        private bool m_lerpToTarget;
        [Tooltip("Curve to apply to the Lerp\nMust be set to enable Lerp")]
        [SerializeField]
        private AnimationCurve m_lerpCurve;
        [Tooltip("Event fired when value of control changes, outputs an INT angle value")]
        [SerializeField]
        private RadialSliderValueChangedEvent _onValueChanged = new RadialSliderValueChangedEvent();
        [Tooltip("Event fired when value of control changes, outputs a TEXT angle value")]
        [SerializeField]
        private RadialSliderTextValueChangedEvent _onTextValueChanged = new RadialSliderTextValueChangedEvent();

        public float Angle
        {
            get { return RadialImage.fillAmount * 360f; }
            set
            {
                if (LerpToTarget)
                {
                    StartLerp(value / 360f);
                }
                else
                {
                    UpdateRadialImage(value / 360f);
                }
            }
        }

        public float Value
        {
            get { return RadialImage.fillAmount; }
            set
            {
                if (LerpToTarget)
                {
                    StartLerp(value);
                }
                else
                {
                    UpdateRadialImage(value);
                }
            }
        }

        public Color EndColor
        {
            get { return m_endColor; }
            set { m_endColor = value; }
        }

        public Color StartColor
        {
            get { return m_startColor; }
            set { m_startColor = value; }
        }

        public bool LerpToTarget
        {
            get { return m_lerpToTarget; }
            set { m_lerpToTarget = value; }
        }

        public AnimationCurve LerpCurve
        {
            get { return m_lerpCurve; }
            set { m_lerpCurve = value; m_lerpTime = LerpCurve[LerpCurve.length - 1].time; }
        }

        public bool LerpInProgress
        {
            get { return lerpInProgress; }
        }

        [Serializable]
        public class RadialSliderValueChangedEvent : UnityEvent<int> { }
        [Serializable]
        public class RadialSliderTextValueChangedEvent : UnityEvent<string> { }

        public Image RadialImage
        {
            get
            {
                if (m_image == null)
                {
                    m_image = GetComponent<Image>();
                    m_image.type = Image.Type.Filled;
                    m_image.fillMethod = Image.FillMethod.Radial360;
                    m_image.fillAmount = 0;
                }
                return m_image;
            }
        }

        public RadialSliderValueChangedEvent onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }
        public RadialSliderTextValueChangedEvent onTextValueChanged
        {
            get { return _onTextValueChanged; }
            set { _onTextValueChanged = value; }
        }

        private void Awake()
        {
            if (LerpCurve != null && LerpCurve.length > 0)
            {
                m_lerpTime = LerpCurve[LerpCurve.length - 1].time;
            }
            else
            {
                m_lerpTime = 1;
            }
        }

        private void Update()
        {
            if (isPointerDown)
            {
                m_targetAngle = GetAngleFromMousePoint();
                if (!lerpInProgress)
                {
                    if (!LerpToTarget)
                    {
                        UpdateRadialImage(m_targetAngle);

                        NotifyValueChanged();
                    }
                    else
                    {
                        if (isPointerReleased) StartLerp(m_targetAngle);
                        isPointerReleased = false;
                    }
                }
            }
            if (lerpInProgress && Value != m_lerpTargetAngle)
            {
                m_currentLerpTime += Time.deltaTime;
                float perc = m_currentLerpTime / m_lerpTime;
                if (LerpCurve != null && LerpCurve.length > 0)
                {
                    UpdateRadialImage(Mathf.Lerp(m_startAngle, m_lerpTargetAngle, LerpCurve.Evaluate(perc)));
                }
                else
                {
                    UpdateRadialImage(Mathf.Lerp(m_startAngle, m_lerpTargetAngle, perc));
                }
            }
            if (m_currentLerpTime >= m_lerpTime || Value == m_lerpTargetAngle)
            {
                lerpInProgress = false;
                UpdateRadialImage(m_lerpTargetAngle);
                NotifyValueChanged();
            }
        }

        private void StartLerp(float targetAngle)
        {
            if (!lerpInProgress)
            {
                m_startAngle = Value;
                m_lerpTargetAngle = targetAngle;
                m_currentLerpTime = 0f;
                lerpInProgress = true;
            }
        }

        private float GetAngleFromMousePoint()
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, m_screenPos, m_eventCamera, out m_localPos);

            // radial pos of the mouse position.
            return (Mathf.Atan2(-m_localPos.y, m_localPos.x) * 180f / Mathf.PI + 180f) / 360f;
        }

        private void UpdateRadialImage(float targetAngle)
        {
            RadialImage.fillAmount = targetAngle;

            RadialImage.color = Color.Lerp(m_startColor, m_endColor, targetAngle);
        }

        private void NotifyValueChanged()
        {
            _onValueChanged.Invoke((int)(m_targetAngle * 360f));
            _onTextValueChanged.Invoke(((int)(m_targetAngle * 360f)).ToString());
        }

//#if UNITY_EDITOR

//        private void OnValidate()
//        {
//            if (LerpToTarget && LerpCurve.length < 2)
//            {
//                LerpToTarget = false;
//                Debug.LogError("You need to define a Lerp Curve to enable 'Lerp To Target'");
//            }
//        }
//#endif

        #region Interfaces
        // Called when the pointer enters our GUI component.
        // Start tracking the mouse
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_screenPos = eventData.position;
            m_eventCamera = eventData.enterEventCamera;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_screenPos = eventData.position;
            m_eventCamera = eventData.enterEventCamera;
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_screenPos = Vector2.zero;
            isPointerDown = false;
            isPointerReleased = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_screenPos = eventData.position;
        }
        #endregion
    }
}