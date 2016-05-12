/// Credit Martin Sharkbomb 
/// Sourced from - http://www.sharkbombs.com/2015/08/26/unity-ui-scrollrect-tools/

using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ScrollRectTweener")]
    public class ScrollRectTweener : MonoBehaviour, IDragHandler
    {

        ScrollRect scrollRect;
        Vector2 startPos;
        Vector2 targetPos;

        bool wasHorizontal;
        bool wasVertical;

        public float moveSpeed = 5000f;
        public bool disableDragWhileTweening = false;

        void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            wasHorizontal = scrollRect.horizontal;
            wasVertical = scrollRect.vertical;
        }

        public void ScrollHorizontal(float normalizedX)
        {
            Scroll(new Vector2(normalizedX, scrollRect.verticalNormalizedPosition));
        }

        public void ScrollHorizontal(float normalizedX, float duration)
        {
            Scroll(new Vector2(normalizedX, scrollRect.verticalNormalizedPosition), duration);
        }

        public void ScrollVertical(float normalizedY)
        {
            Scroll(new Vector2(scrollRect.horizontalNormalizedPosition, normalizedY));
        }

        public void ScrollVertical(float normalizedY, float duration)
        {
            Scroll(new Vector2(scrollRect.horizontalNormalizedPosition, normalizedY), duration);
        }

        public void Scroll(Vector2 normalizedPos)
        {
            Scroll(normalizedPos, GetScrollDuration(normalizedPos));
        }

        float GetScrollDuration(Vector2 normalizedPos)
        {
            Vector2 currentPos = GetCurrentPos();
            return Vector2.Distance(DeNormalize(currentPos), DeNormalize(normalizedPos)) / moveSpeed;
        }

        Vector2 DeNormalize(Vector2 normalizedPos)
        {
            return new Vector2(normalizedPos.x * scrollRect.content.rect.width, normalizedPos.y * scrollRect.content.rect.height);
        }

        Vector2 GetCurrentPos()
        {
            return new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
        }

        public void Scroll(Vector2 normalizedPos, float duration)
        {
            startPos = GetCurrentPos();
            targetPos = normalizedPos;

            if (disableDragWhileTweening)
                LockScrollability();

            StopAllCoroutines();
            StartCoroutine(DoMove(duration));
        }

        IEnumerator DoMove(float duration)
        {

            // Abort if movement would be too short
            if (duration < 0.05f)
                yield break;

            Vector2 posOffset = targetPos - startPos;

            float currentTime = 0f;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                scrollRect.normalizedPosition = EaseVector(currentTime, startPos, posOffset, duration);
                yield return null;
            }

            scrollRect.normalizedPosition = targetPos;

            if (disableDragWhileTweening)
                RestoreScrollability();
        }

        public Vector2 EaseVector(float currentTime, Vector2 startValue, Vector2 changeInValue, float duration)
        {
            return new Vector2(
                changeInValue.x * Mathf.Sin(currentTime / duration * (Mathf.PI / 2)) + startValue.x,
                changeInValue.y * Mathf.Sin(currentTime / duration * (Mathf.PI / 2)) + startValue.y
                );
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!disableDragWhileTweening)
                StopScroll();
        }

        void StopScroll()
        {
            StopAllCoroutines();
            if (disableDragWhileTweening)
                RestoreScrollability();
        }

        void LockScrollability()
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
        }

        void RestoreScrollability()
        {
            scrollRect.horizontal = wasHorizontal;
            scrollRect.vertical = wasVertical;
        }

    }
}