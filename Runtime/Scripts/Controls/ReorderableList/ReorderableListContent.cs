/// Credit Ziboo
/// Sourced from - http://forum.unity3d.com/threads/free-reorderable-list.364600/

using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [DisallowMultipleComponent]
    public class ReorderableListContent : MonoBehaviour
    {
        private List<Transform> _cachedChildren;
        private List<ReorderableListElement> _cachedListElement;
        private ReorderableListElement _ele;
        private ReorderableList _extList;
        private RectTransform _rect;
        private Coroutine _refreshCoroutine;

        private void OnEnable()
        {
            if (_rect) RestartRefresh();
        }

        public void OnTransformChildrenChanged()
        {
            if (this.isActiveAndEnabled) RestartRefresh();
        }

        public void Init(ReorderableList extList)
        {
            _extList = extList;
            _rect = GetComponent<RectTransform>();
            _cachedChildren = new List<Transform>();
            _cachedListElement = new List<ReorderableListElement>();

            RestartRefresh();
        }

        // Restart the child-refresh coroutine, stopping any in-flight run first.
        // StopCoroutine(RefreshChildren()) does NOT work: it allocates a fresh enumerator
        // that was never started, so the running coroutine is never cancelled and multiple
        // refreshes can stack up (e.g. during a drag that reparents children each frame).
        private void RestartRefresh()
        {
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }
            if (_rect != null && isActiveAndEnabled)
            {
                _refreshCoroutine = StartCoroutine(RefreshChildren());
            }
        }

        private IEnumerator RefreshChildren()
        {
            //Handle new children
            for (int i = 0; i < _rect.childCount; i++)
            {
                if (_cachedChildren.Contains(_rect.GetChild(i)))
                    continue;

                //Get or Create ReorderableListElement
                _ele = _rect.GetChild(i).gameObject.GetComponent<ReorderableListElement>() ??
                       _rect.GetChild(i).gameObject.AddComponent<ReorderableListElement>();
                _ele.Init(_extList);

                _cachedChildren.Add(_rect.GetChild(i));
                _cachedListElement.Add(_ele);
            }

            //HACK a little hack, if I don't wait one frame I don't have the right deleted children
            yield return 0;

            //Remove deleted child
            for (int i = _cachedChildren.Count - 1; i >= 0; i--)
            {
                if (_cachedChildren[i] == null)
                {
                    _cachedChildren.RemoveAt(i);
                    _cachedListElement.RemoveAt(i);
                }
            }
        }
    }
}