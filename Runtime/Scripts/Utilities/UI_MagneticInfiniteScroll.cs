/// Credit Febo Zodiaco
/// Sourced from - https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/issues/349/magnticinfinitescroll
/// 

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/UI Magnetic Infinite Scroll")]
    public class UI_MagneticInfiniteScroll : UI_InfiniteScroll
    {
        public event Action<GameObject> OnNewSelect;

        [Tooltip("The pointer to the pivot, the visual element for centering objects.")]
        [SerializeField]
        private RectTransform pivot = null;
        [Tooltip("The pointer to the object container")]
        [SerializeField]
        private RectTransform content = null;
        [Tooltip("the maximum speed that allows you to activate the magnet to center on the pivot")]
        [SerializeField]
        private float maxSpeedForMagnetic = 10f;
        [SerializeField]
        [Tooltip("The initial index of the object which must be initially centered")]
        private int indexStart = 0;
        [SerializeField]
        [Tooltip("The time to decelerate and aim for the pivot")]
        private float timeForDeceleration = 0.05f;

        [SerializeField]
        private ScrollRect scrollRect = null;

        private float _pastPositionMouseSpeed;
        private float initMovementDirection = 0;
        private float _pastPosition = 0;

        private float _currentSpeed = 0.0f;
        private float _stopValue = 0.0f;
        private readonly float _waitForContentSet = 0.1f;
        private float _currentTime = 0;
        private int nearestIndex = 0;

        private bool _useMagnetic = true;
        private bool _isStopping = false;
        private bool _isMovement = false;

        public List<RectTransform> Items { get; }

        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(SetInitContent());
        }

        private void Update()
        {
            if (!content || !pivot || !_useMagnetic || !_isMovement || items == null || scrollRect == null)
            {
                return;
            }

            float currentPosition = GetRightAxis(content.anchoredPosition);
            _currentSpeed = Mathf.Abs(currentPosition - _pastPosition);
            _pastPosition = currentPosition;
            if (Mathf.Abs(_currentSpeed) > maxSpeedForMagnetic)
            {
                return;
            }

            if (_isStopping)
            {
                Vector2 anchoredPosition = content.anchoredPosition;
                _currentTime += Time.deltaTime;
                float valueLerp = _currentTime / timeForDeceleration;

                float newPosition = Mathf.Lerp(GetRightAxis(anchoredPosition), _stopValue, valueLerp);

                content.anchoredPosition = _isVertical ? new Vector2(anchoredPosition.x, newPosition) :
                                new Vector2(newPosition, anchoredPosition.y);


                if (newPosition == GetRightAxis(anchoredPosition) && nearestIndex > 0 && nearestIndex < items.Count)
                {
                    _isStopping = false;
                    _isMovement = false;
                    var item = items[nearestIndex];
                    if (item != null && OnNewSelect != null)
                    {

                        OnNewSelect.Invoke(item.gameObject);
                    }
                }
            }
            else
            {
                float distance = Mathf.Infinity * (-initMovementDirection);

                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item == null)
                    {
                        continue;
                    }

                    var aux = GetRightAxis(item.position) - GetRightAxis(pivot.position);

                    if ((initMovementDirection <= 0 && aux < distance && aux > 0) ||
                        (initMovementDirection > 0 && aux > distance && aux < 0))
                    {
                        distance = aux;
                        nearestIndex = i;
                    }
                }

                _isStopping = true;
                _stopValue = GetAnchoredPositionForPivot(nearestIndex);
                scrollRect.StopMovement();
            }
        }

        public void Drag()
        {
            float currentPosition = GetRightAxis(UIExtensionsInputManager.MousePosition);

            initMovementDirection = Mathf.Sign(currentPosition - _pastPositionMouseSpeed);
            _pastPositionMouseSpeed = currentPosition;
            _useMagnetic = false;
            _isStopping = false;
        }

        public void EndDrag()
        {
            FinishPrepareMovement();
        }

        public override void SetNewItems(ref List<Transform> newItems)
        {
            foreach (var element in newItems)
            {
                RectTransform rectTransform = element.GetComponent<RectTransform>();
                if (rectTransform && pivot)
                {
                    rectTransform.sizeDelta = pivot.sizeDelta;
                }
            }
            base.SetNewItems(ref newItems);
        }

        public void Scroll()
        {
            initMovementDirection = -UIExtensionsInputManager.MouseScrollDelta.y;
            FinishPrepareMovement();
        }

        public void SetContentInPivot(int index)
        {
            float newPos = GetAnchoredPositionForPivot(index);
            Vector2 anchoredPosition = content.anchoredPosition;

            if (content)
            {
                content.anchoredPosition = _isVertical ? new Vector2(anchoredPosition.x, newPos) :
                                            new Vector2(newPos, anchoredPosition.y);
                _pastPosition = GetRightAxis(content.anchoredPosition);
            }
        }

        private IEnumerator SetInitContent()
        {
            yield return new WaitForSeconds(_waitForContentSet);
            SetContentInPivot(indexStart);
        }

        private float GetAnchoredPositionForPivot(int index)
        {
            if (!pivot || items == null || items.Count < 0)
            {
                return 0f;
            }

            index = Mathf.Clamp(index, 0, items.Count - 1);

            float posItem = GetRightAxis(items[index].anchoredPosition);
            float posPivot = GetRightAxis(pivot.anchoredPosition);
            return posPivot - posItem;
        }

        private void FinishPrepareMovement()
        {
            _isMovement = true;
            _useMagnetic = true;
            _isStopping = false;
            _currentTime = 0;
        }

        private float GetRightAxis(Vector2 vector)
        {
            return _isVertical ? vector.y : vector.x;
        }
    }
}
